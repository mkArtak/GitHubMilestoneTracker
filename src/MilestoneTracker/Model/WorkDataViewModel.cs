using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MilestoneTracker.Model
{
    public class WorkDataViewModel
    {
        private string[] _milestones;

        private string[] _members;

        private readonly Lazy<double> totalWorkAmountCache;

        public IDictionary<string, IDictionary<string, MemberMilestoneData>> WorkPerMember { get; set; } = new ConcurrentDictionary<string, IDictionary<string, MemberMilestoneData>>();

        public string[] Milestones { get => _milestones ?? (_milestones = this.WorkPerMember.Values.SelectMany(item => item.Keys).Distinct().OrderBy(item => item).ToArray()); }

        public string[] Members { get => _members ?? (_members = this.WorkPerMember.Keys.OrderBy(item => item).ToArray()); }

        public double TotalAmountOfWork { get => this.totalWorkAmountCache.Value; }

        public double this[string memberName, string milestone]
        {
            get => this?.WorkPerMember?[memberName]?[milestone]?.AmountOfWork ?? 0;
            set => this.AddMemberWorkForMilestone(memberName, milestone, value);
        }

        public double this[string memberName]
        {
            get => this.WorkPerMember[memberName].Sum(item => item.Value.AmountOfWork);
        }

        public WorkDataViewModel()
        {
            this.totalWorkAmountCache = new Lazy<double>(() =>
            {
                return this.Members.Sum(m => this[m]);
            });
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
