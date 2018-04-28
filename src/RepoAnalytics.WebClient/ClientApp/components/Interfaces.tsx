interface TeamMember {
    name: string;
    includeInReports: boolean;
}

interface CostMarker {
    name: string;
    factor: Number;
}

interface TeamInfo {
    name: string;
    organization: string;
    teamMembers: TeamMember[];
    costLabels: CostMarker[];
    defaultMilestoneToTrack: string;
    repositories: string[]
}

export {TeamMember, TeamInfo, CostMarker};