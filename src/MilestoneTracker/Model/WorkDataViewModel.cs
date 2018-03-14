using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MilestoneTracker.Model
{
    public class WorkDataViewModel
    {
        private string[] _milestones;

        private string[] _members;

        public IDictionary<string, IDictionary<string, MemberMilestoneData>> WorkPerMember { get; set; } = new ConcurrentDictionary<string, IDictionary<string, MemberMilestoneData>>();

        public string[] Milestones { get => _milestones ?? (_milestones = this.WorkPerMember.Values.SelectMany(item => item.Keys).Distinct().OrderBy(item => item).ToArray()); }

        public string[] Members { get => _members ?? (_members = this.WorkPerMember.Keys.OrderBy(item => item).ToArray()); }

        public double this[string memberName, string milestone]
        {
            get => this?.WorkPerMember?[memberName]?[milestone]?.AmountOfWork ?? 0;
            set => this.AddMemberWorkForMilestone(memberName, milestone, value);
        }

        private void AddMemberWorkForMilestone(string member, string milestone, double amountOfWork)
        {
            if (!this.WorkPerMember.ContainsKey(member))
            {
                this.WorkPerMember[member] = new ConcurrentDictionary<string, MemberMilestoneData>();
            }

            if (!this.WorkPerMember[member].ContainsKey(milestone))
            {
                this.WorkPerMember[member][milestone] = new MemberMilestoneData
                {
                    MemberName = member,
                    Milestone = milestone,
                    AmountOfWork = 0
                };
            }

            this.WorkPerMember[member][milestone].AmountOfWork += amountOfWork;
        }
    }
}
