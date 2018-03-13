using AM.Common.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GitHub.Client
{
    internal class IssuesQueryHandler : IIssuesQueryHandler
    {
        private readonly HttpClient client;

        public IssuesQueryHandler(HttpClient client)
        {
            this.client = client.Ensure(nameof(client)).IsNotNull().Value;
        }

        public async Task<IEnumerable<Issue>> QueryAsync(string assignee, string milestone, CancellationToken cancellationToken)
        {
            GitHubApiRequestUrlBuilder uriBuilder = new GitHubApiRequestUrlBuilder();
            uriBuilder.Milestone = milestone;
            uriBuilder.Assignee = assignee;

            HttpRequestMessage requst = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Build());
            HttpResponseMessage response = await this.client.GetAsync(uriBuilder.Build(), cancellationToken);

            string content = await (response.Content as StringContent).ReadAsStringAsync();
            IssuesQueryResult response = JsonSerializer.Create().Deserialize<IssuesQueryResult>(response.Content.ReadAsStreamAsync());
        }
    }
}
