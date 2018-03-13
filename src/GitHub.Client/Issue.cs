using System.Collections.Generic;

namespace GitHub.Client
{
    public class Issue
    {
        public string Url { get; set; }

        public Member Assignee { get; set; }

        public IEnumerable<Member> Assignees { get; set; }

        public int Id { get; set; }

        public Milestone Milestone { get; set; }

        public int Number { get; set; }

        public string Title { get; set; }

        public IEnumerable<Label> Labels { get; set; }
    }
}
