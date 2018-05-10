import {ICostMarker} from "./ICostMarker";
import {ITeamMember} from "./ITeamMember";

interface ITeamInfo {
    name: string;
    organization: string;
    teamMembers: ITeamMember[];
    costLabels: ICostMarker[];
    defaultMilestonesToTrack: string;
    repositories: string[];
}

export {ITeamInfo};