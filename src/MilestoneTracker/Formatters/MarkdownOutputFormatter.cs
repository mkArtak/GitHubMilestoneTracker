using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using MilestoneTracker.ViewModels;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MilestoneTracker.Formatters
{
    public class MarkdownOutputFormatter : TextOutputFormatter
    {
        public MarkdownOutputFormatter()
        {
            this.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/markdown"));

            this.SupportedEncodings.Add(Encoding.UTF8);
            this.SupportedEncodings.Add(Encoding.Unicode);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            var logger = serviceProvider.GetService(typeof(ILogger<MarkdownOutputFormatter>)) as ILogger;
            var response = context.HttpContext.Response;
            response.Headers.Add("Content-Disposition", "attachment; filename=Report.md");

            var buffer = new StringBuilder();
            var person = context.Object as MergedPRsViewModel;
            await FormatAsTableAsync(buffer, person, logger);
            await response.WriteAsync(buffer.ToString());
        }

        protected override bool CanWriteType(Type type)
        {
            if (typeof(MergedPRsViewModel).IsAssignableFrom(type))
            {
                return base.CanWriteType(type);
            }

            return false;
        }

        private static async Task FormatAsTableAsync(StringBuilder buffer, MergedPRsViewModel prs, ILogger logger)
        {
            await GenerateMarkdownTableForPrs(buffer, "Community PRs", prs.CommunityPRs);
            await GenerateMarkdownTableForPrs(buffer, "Team PRs", prs.InternalPRs);
        }

        private static async Task GenerateMarkdownTableForPrs(StringBuilder buffer, string header, PRListViewModel list)
        {
            AppendHeader(buffer, header);
            await AppendRowsAsync(buffer, list);
            buffer.AppendLine();
        }

        private static async Task AppendRowsAsync(StringBuilder builder, PRListViewModel prs)
        {
            foreach (var pr in prs.PullRequests)
            {
                var link = await prs.IconRetriever.GetUserProfileIconUrl(pr.CreatorLogin);
                builder.AppendLine($"| ![icon]({link}&s=20) {pr.CreatorLogin} | [{pr.Number}]({pr.Url}) | {pr.Title}");
            }
        }

        private static StringBuilder AppendHeader(StringBuilder buffer, string header)
        {
            buffer.AppendLine($"### {header}");
            buffer.AppendLine();
            buffer.AppendLine("| Author | PR | Title");
            buffer.AppendLine("| - | - | - ");

            return buffer;
        }
    }
}
