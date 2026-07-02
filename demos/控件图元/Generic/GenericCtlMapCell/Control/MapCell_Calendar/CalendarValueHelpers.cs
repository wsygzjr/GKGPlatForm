using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace GKG.Map.MapCell.Generic.Control.MapCell_Calendar
{
    internal sealed class CalendarBlackoutRangeDto
    {
        public string Start { get; set; } = string.Empty;

        public string End { get; set; } = string.Empty;
    }

    internal static class CalendarValueHelpers
    {
        // 绑定、序列化、事件参数统一使用这个日期字符串格式。
        public const string DateFormat = "yyyy-MM-dd";

        public static string FormatDate(DateTime? value)
        {
            return value.HasValue
                ? value.Value.Date.ToString(DateFormat, CultureInfo.InvariantCulture)
                : string.Empty;
        }

        public static bool TryParseDate(string? text, out DateTime value)
        {
            return DateTime.TryParseExact(
                text ?? string.Empty,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out value);
        }

        public static List<DateTime> ParseDateList(string? json)
        {
            // SelectedDates 约定为 ["2026-04-01","2026-04-02"] 这样的字符串数组。
            List<DateTime> dates = new();
            if (string.IsNullOrWhiteSpace(json))
                return dates;

            try
            {
                using JsonDocument jsonDocument = JsonDocument.Parse(json);
                if (jsonDocument.RootElement.ValueKind != JsonValueKind.Array)
                    return dates;

                foreach (JsonElement item in jsonDocument.RootElement.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String && TryParseDate(item.GetString(), out DateTime date))
                        dates.Add(date.Date);
                }
            }
            catch
            {
            }

            return dates
                .Distinct()
                .OrderBy(item => item)
                .ToList();
        }

        public static string SerializeDateList(IEnumerable<DateTime> dates)
        {
            var normalized = dates?
                .Select(item => item.Date)
                .Distinct()
                .OrderBy(item => item)
                .Select(item => item.ToString(DateFormat, CultureInfo.InvariantCulture))
                .ToList()
                ?? new List<string>();

            return JsonSerializer.Serialize(normalized);
        }

        public static List<CalendarBlackoutRangeDto> ParseBlackoutRanges(string? json)
        {
            // BlackoutDates 约定为 [{"Start":"2026-04-01","End":"2026-04-03"}] 这样的区间数组。
            List<CalendarBlackoutRangeDto> ranges = new();
            if (string.IsNullOrWhiteSpace(json))
                return ranges;

            try
            {
                using JsonDocument jsonDocument = JsonDocument.Parse(json);
                if (jsonDocument.RootElement.ValueKind != JsonValueKind.Array)
                    return ranges;

                foreach (JsonElement item in jsonDocument.RootElement.EnumerateArray())
                {
                    if (item.ValueKind != JsonValueKind.Object)
                        continue;

                    string start = item.TryGetProperty(nameof(CalendarBlackoutRangeDto.Start), out JsonElement startElement) && startElement.ValueKind == JsonValueKind.String
                        ? startElement.GetString() ?? string.Empty
                        : string.Empty;
                    string end = item.TryGetProperty(nameof(CalendarBlackoutRangeDto.End), out JsonElement endElement) && endElement.ValueKind == JsonValueKind.String
                        ? endElement.GetString() ?? string.Empty
                        : string.Empty;

                    if (!TryParseDate(start, out DateTime startDate))
                        continue;
                    if (!TryParseDate(string.IsNullOrWhiteSpace(end) ? start : end, out DateTime endDate))
                        continue;

                    if (endDate < startDate)
                        (startDate, endDate) = (endDate, startDate);

                    // 统一规整成 Start <= End 的标准区间，避免后续再做额外判断。
                    ranges.Add(new CalendarBlackoutRangeDto
                    {
                        Start = FormatDate(startDate),
                        End = FormatDate(endDate)
                    });
                }
            }
            catch
            {
            }

            return ranges;
        }

        public static string SerializeBlackoutRanges(IEnumerable<CalendarBlackoutRangeDto>? ranges)
        {
            List<CalendarBlackoutRangeDto> normalized = new();
            if (ranges != null)
            {
                foreach (CalendarBlackoutRangeDto range in ranges)
                {
                    if (!TryParseDate(range?.Start, out DateTime startDate))
                        continue;
                    if (!TryParseDate(string.IsNullOrWhiteSpace(range?.End) ? range?.Start : range?.End, out DateTime endDate))
                        continue;

                    if (endDate < startDate)
                        (startDate, endDate) = (endDate, startDate);

                    // 写回前再次做标准化，保证持久化格式稳定。
                    normalized.Add(new CalendarBlackoutRangeDto
                    {
                        Start = FormatDate(startDate),
                        End = FormatDate(endDate)
                    });
                }
            }

            return JsonSerializer.Serialize(normalized);
        }
    }
}
