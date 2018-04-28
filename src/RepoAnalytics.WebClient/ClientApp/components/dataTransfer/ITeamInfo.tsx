import {ICostMarker} from './ICostMarker';
import {ITeamMember} from './ITeamMember';

interface ITeamInfo {
    name: string;
    organization: string;
    teamMembers: ITeamMember[];
    costLabels: ICostMarker[];
    defaultMilestoneToTrack: string;
    repositories: string[]
}

export {ITeamInfo};