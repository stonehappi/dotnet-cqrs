using System.Text.Json;
using ClosedXML.Excel;

namespace DotnetCqrs.Infrastructure.Data;

public interface IExcelService<T>
{
	public byte[] WriteExcel(IReadOnlyList<T> items, Dictionary<string, object>? data = null);
	public List<T> GetItems(IFormFile file);
}

public class ExcelService<T> : IExcelService<T>
{
	private readonly List<ExcelBase> _columns;

	protected ExcelService(List<ExcelBase> columns)
	{
		_columns = columns;
		for (var i = 0; i < _columns.Count; i++)
		{
			_columns[i].Index = i;
		}
	}

	private bool CheckHeader(IXLWorksheet worksheet)
	{
		var cols = worksheet.LastColumnUsed()?.ColumnNumber();
		if (cols == null) return false;
		for (var i = 0; i < cols; i++)
		{
			var header = worksheet.Cell(1, i + 1).Value.ToString();
			var index = _columns.FindIndex(column =>
				column.Name.Equals(header, StringComparison.CurrentCultureIgnoreCase));
			if (index == -1) return false;
			_columns[index].Index = i;
		}

		return true;
	}

	private void WriteHeader(IXLWorksheet worksheet)
	{
		foreach (var excelBase in _columns)
		{
			excelBase.WriteHeader(worksheet);
		}
	}

	public byte[] WriteExcel(IReadOnlyList<T> items, Dictionary<string, object>? data)
	{
		using var file = new MemoryStream();
		var workbook = new XLWorkbook();
		var worksheet = workbook.AddWorksheet("Data");
		WriteHeader(worksheet);
		if (data != null)
		{
			foreach (var (key, value) in data)
			{
				var excelBase = _columns.Find(f => f.Property == key);
				if (excelBase == null) throw new Exception("Header is not existed");
				excelBase.SetDropdown(workbook, worksheet, value as string[] ?? []);
			}
		}

		for (var row = 0; row < items.Count; row++)
		{
			var item = items[row];
			var dict = item?.GetType()
				.GetProperties()
				.ToDictionary(prop => prop.Name, prop => prop.GetValue(item, null));
			foreach (var excelBase in _columns)
			{
				var key = excelBase.Property ?? excelBase.Name;
				excelBase.Write(worksheet, row + 2, dict?[key]);
			}
		}

		workbook.SaveAs(file);
		return file.ToArray();
	}

	public List<T> GetItems(IFormFile file)
	{
		return ReadExcel(file.OpenReadStream());
	}

	private List<T> ReadExcel(Stream file)
	{
		using var workbook = new XLWorkbook(file);
		var worksheet = workbook.Worksheet(1);
		if (!CheckHeader(worksheet)) throw new Exception("Header is not valid");
		var items = new List<T>();
		var rows = worksheet.LastRowUsed()?.RowNumber();
		for (var row = 2; row <= rows; row++)
		{
			var item = _read(worksheet, row);
			if (item == null) throw new Exception($"Row {row} is not valid");
			items.Add(item);
		}

		return items;
	}

	private T? _read(IXLWorksheet worksheet, int row)
	{
		var dict = new Dictionary<string, object>();
		foreach (var column in _columns)
		{
			var value = column.Read(worksheet, row);
			var key = column.Property ?? column.Name;
			dict.Add(key, value);
		}

		var json = JsonSerializer.Serialize(dict);
		return JsonSerializer.Deserialize<T>(json);
	}
}

public class ExcelBase
{
	public int Index { get; set; }
	public string Name { get; set; } = null!;

	public string? Property { get; set; }
	private EExcelType Type { get; set; } = EExcelType.Text;

	public void WriteHeader(IXLWorksheet worksheet)
	{
		worksheet.Cell(1, Index + 1).Value = Name;
	}

	public void Write(IXLWorksheet worksheet, int row, dynamic? value)
	{
		if (row < 2) return;
		worksheet.Cell(row, Index + 1).SetValue(value?.ToString());
	}

	public dynamic Read(IXLWorksheet worksheet, int row)
	{
		return Type switch
		{
			EExcelType.Dropdown => worksheet.Cell(row, Index + 1).GetValue<string>(),
			EExcelType.Int => _readInt(worksheet, row),
			EExcelType.Double => _readDouble(worksheet, row),
			EExcelType.Date => worksheet.Cell(row, Index + 1).GetValue<DateTime>(),
			_ => worksheet.Cell(row, Index + 1).GetValue<string>()
		};
	}

	private int _readInt(IXLWorksheet worksheet, int row)
	{
		var value = worksheet.Cell(row, Index + 1).GetValue<string>();
		return int.TryParse(value, out var result) ? result : 0;
	}

	private double _readDouble(IXLWorksheet worksheet, int row)
	{
		var value = worksheet.Cell(row, Index + 1).GetValue<string>();
		return double.TryParse(value, out var result) ? result : 0;
	}


	public void SetDropdown(IXLWorkbook workbook, IXLWorksheet worksheet, string[] items)
	{
		var hiddenSheet = workbook.AddWorksheet(Name);
		for (var i = 1; i <= items.Length; i++)
		{
			hiddenSheet.Cell($"A{i}").SetValue(items[i - 1]);
		}

		worksheet
			.Range(2, Index, 10000, Index)
			.CreateDataValidation()
			.List(hiddenSheet.Range($"A1:A{items.Length + 1}"), true);
	}
}

public enum EExcelType
{
	Text,
	Int,
	Double,
	Date,
	Dropdown
}