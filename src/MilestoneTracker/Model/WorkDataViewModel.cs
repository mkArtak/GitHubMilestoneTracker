using System;
using System.Collections.Generic;
using System.Linq;

namespace MilestoneTracker.Model
{
    public class WorkDataViewModel
    {
        private string[] _milestones;
        private string[] _members;

        private readonly Lazy<double> totalWorkAmountCache;
        private readonly Lazy<string[]> membersWithMaxAmountOfWorkCache;
        private readonly Lazy<string[]> membersWithMinAmountOfWorkCache;

        private IDictionary<string, IDictionary<string, MemberMilestoneData>> workPerMember { get; set; } = new Dictionary<string, IDictionary<string, MemberMilestoneData>>();
        private IDictionary<string, double> amountOfWorkPerMember = new Dictionary<string, double>();

        public string[] Milestones { get => _milestones ?? (_milestones = this.workPerMember.Values.SelectMany(item => item.Keys).Distinct().OrderBy(item => item).ToArray()); }

        public string[] Members { get => _members ?? (_members = this.workPerMember.Keys.OrderBy(item => item).ToArray()); }

        public double TotalAmountOfWork { get => this.totalWorkAmountCache.Value; }

        public double this[string memberName, string milestone]
        {
            get
            {
                if (this.workPerMember == null
                    || !this.workPerMember.TryGetValue(memberName, out IDictionary<string, MemberMilestoneData> memberWork)
                    || !memberWork.TryGetValue(milestone, out MemberMilestoneData milestoneData))
                {
                    return 0;
                }

                return milestoneData.AmountOfWork;
            }
            set => this.AddMemberWorkForMilestone(memberName, milestone, value);
        }

        public double this[string memberName]
        {
            get => this.amountOfWorkPerMember[memberName];
        }

        public string[] MembersWithMaxAmountOfWork
        {
            get => this.membersWithMaxAmountOfWorkCache.Value;
        }

        public string[] MembersWithMinAmountOfWork
        {
            get => this.membersWithMinAmountOfWorkCache.Value;
        }

        public string TeamName { get; set; }

        public WorkDataViewModel()
        {
            this.totalWorkAmountCache = new Lazy<double>(() =>
            {
                return this.Members.Sum(m => this[m]);
            });

            this.membersWithMaxAmountOfWorkCache = new Lazy<string[]>(() =>
            {
                double maxAmountOfWork = this.amountOfWorkPerMember.Max(item => item.Value);
                return this.amountOfWorkPerMember.Where(item => item.Value == maxAmountOfWork).Select(item => item.Key).ToArray();
            });

            this.membersWithMinAmountOfWorkCache = new Lazy<string[]>(() =>
            {
                double minAmountOfWork = this.amountOfWorkPerMember.Min(item => item.Value);
                return this.amountOfWorkPerMember.Where(item => item.Value == minAmountOfWork).Select(item => item.Key).ToArray();
            });
        }

        public string GetClassForMemberCell(string member)
        {
            if (this.MembersWithMaxAmountOfWork.Contains(member))
            {
                return " row-item-max";
            }
            else if (this.MembersWithMinAmountOfWork.Contains(member))
            {
                return " row-item-min";
            }

            return string.Empty;
        }

        private void AddMemberWorkForMilestone(string member, string milestone, double amountOfWork)
        {
            if (!this.workPerMember.ContainsKey(member))
            {
                this.workPerMember[member] = new Dictionary<string, MemberMilestoneData>();
                this.amountOfWorkPerMember.Add(member, 0);
            }

            if (!this.workPerMember[member].ContainsKey(milestone))
            {
                this.workPerMember[member][milestone] = new MemberMilestoneData
                {
                    MemberName = member,
                    Milestone = milestone,
                    AmountOfWork = 0
                };
            }

            this.workPerMember[member][milestone].AmountOfWork += amountOfWork;
            this.amountOfWorkPerMember[member] += amountOfWork;
        }
    }
}
