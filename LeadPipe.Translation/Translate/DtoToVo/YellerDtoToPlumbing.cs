using LeadPipe.Domain.ValueObjects;
using LeadPipe.Infrastructure.Dto;
using LeadPipe.Infrastructure.Interfaces.Translate;
using System.Data;

namespace LeadPipe.Translation.Translate.DtoToVo;

internal partial class YellerDtoToPlumbing : IDtoToVo<YellerDto, Plumbing>
{
    public Plumbing Translate(YellerDto data)
    {
        // Find Phone Number
        HashSet<long> seen = [];
        var collectedNumbers = (data.project?.survey_answers?
            .OfType<SurveyAnswer>()
            .SelectMany(ans => ans.answer_text ?? Array.Empty<string>()) // flatten all answer_text arrays
            .SelectMany(raw =>
                PhoneNumber.TryParseMany(raw, out var nums)
                    ? nums.Where(n => seen.Add(n.Number)) // only keep new numbers
                    : []
            )
            .ToList()
        ) ?? [];

        PhoneNumber canonicalPhoneNumber = collectedNumbers.Count == 0
            ? PhoneNumber.DefaultPhoneNumber
            : collectedNumbers[^1];


        // Date
        DateTimeOffset date =
            data.time_created is DateTime dtc
                ? new(DateTime.SpecifyKind(dtc, DateTimeKind.Utc), TimeSpan.Zero)
                : DateTimeOffset.MaxValue;

        // Contents
        IEnumerable<string> contentsStr = [];
        if (data.project?.survey_answers is SurveyAnswer[] surveyAnswers)
            contentsStr = surveyAnswers.Select(a =>
            {
                string question = a.question_text ?? "(Question text missing)";
                string[] answers = a.answer_text ?? [];

                return string.Join(" | ",
                    answers.Length == 0
                        ? [question, "(Answer missing)"]
                        : [question, .. answers]
                );
            });
        else contentsStr = [""];

        string contents = string.Join(" <|> ", contentsStr);

        string metadata = string.Empty;
        return new(
            Id: 0,
            PhoneNumber: canonicalPhoneNumber,
            Date: date,
            Contents: contents,
            Branch: null,
            MetaData: metadata,
            Source.Yeller,
            Numbers: [.. collectedNumbers]
        );
    }

}
