using System.Linq;
using AutoMapper;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.ResolvedIssues
{
    public class IssueMlCsvProfile : Profile
    {
        public IssueMlCsvProfile()
        {
            CreateMap<Issue, IssueMlCsv>()
                .ForMember(ml => ml.TagsConcatenated,
                    opt => opt.MapFrom(source => string.Join(",", source.Tags.Select(x => x.Name))))
                .ForMember(ml => ml.ProjectName, opt => opt.MapFrom(source => source.Project.Name))
                .ForMember(ml => ml.AssigneeLogin, opt => opt.MapFrom(source => source.Assignee.Login))
                .ForMember(ml => ml.Complexity, opt => opt.MapFrom(source => source.Complexity.Name))
                .ForMember(ml => ml.EstimationError, opt => opt.MapFrom(s => GetEstimationError(s)))
                .ForMember(ml => ml.SuccessGrade, opt => opt.MapFrom(source => source.SuccessGrade.Name))
                .ForMember(ml => ml.IssueType, opt => opt.MapFrom(source => source.IssueType.Name))
                .ForMember(ml => ml.ReviewRefuses, opt => opt.MapFrom(source => CountReviewRefuses(source)))
                .ForMember(ml => ml.TestRefuses, opt => opt.MapFrom(source => CountTestRefuses(source)))
                ;
        }

        private static double? GetEstimationError(Issue source)
        {
            return (double?)source.Spent.Minutes / source.Estimate.Minutes - 1;
        }

        private static int CountReviewRefuses(Issue source)
        {
            var r = source.Changelog.History.Count(item => item.Author.Id != source.Assignee.Id
                                                           && item.FromState == "Review"
                                                           && item.ToState == "Incomplete");
            return r;
        }
        private static int CountTestRefuses(Issue source)
        {
            var r = source.Changelog.History.Count(item => item.Author.Id != source.Assignee.Id
                                                   && item.FromState == "ToTest"
                                                   && item.ToState == "Incomplete");
            return r;
        }
    }
}