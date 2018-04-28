import * as React from 'react';
import 'isomorphic-fetch';
import { TeamMember, TeamInfo } from './Interfaces';

interface ITeamMembersProps {
    teamName: string;
    members: TeamMember[];
}

interface IteamMembersState {
    members: TeamMember[];
    loading: boolean;
}

export class TeamMembers extends React.Component<ITeamMembersProps, IteamMembersState> {
    constructor(props: ITeamMembersProps) {
        super(props);

        this.state = {
            members: props.members,
            loading: false
        };
    }

    public render() {
        let memberNodes = this.state.members.map(function (member: TeamMember) {
            return (
                <div className="row">
                    <div className="col-sm-9">{member.name}</div>
                    <div className="col-sm-1">
                        <input type="button" className="btn btn-warning" value="remove" />
                    </div>
                </div>
            );
        });

        return (
            <div className="panel panel-default section-block" >
                <div className="panel-heading"><h4>Members</h4></div>
                <div className="panel-body">
                    <div className="container-fluid">
                        {memberNodes}
                    </div>
                </div>
                <div className="panel-footer clearfix">
                    <div className="add-item-block">
                        <div className="input-group">
                            <span className="input-group-addon"><i className="glyphicon glyphicon-user"></i></span>
                            <div className="inline-inputs">
                                <input id="txtMemberName" type="text" className="form-control" name="txtMemberName" placeholder="Member name" />
                                <input type="button" id="btnAddMember" name="btnAddMember" className="btn-success" value="Add" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}