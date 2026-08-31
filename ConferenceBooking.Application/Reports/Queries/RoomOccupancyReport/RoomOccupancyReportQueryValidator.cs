using ConferenceBooking.Application.Common.ErrorMessages;
using ConferenceBooking.Application.Reports.Common;
using FluentValidation;

namespace ConferenceBooking.Application.Reports.Queries.RoomOccupancyReport;

public class RoomOccupancyReportQueryValidator : ReportPeriodQueryValidator<RoomOccupancyReportQuery> { }