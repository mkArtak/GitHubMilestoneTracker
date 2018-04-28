import * as React from 'react';
import 'isomorphic-fetch';
import { ITeamMember } from './dataTransfer/ITeamMember';

interface ITeamMembersProps {
    teamName: string;
    members: ITeamMember[];
}

interface IteamMembersState {
    members: ITeamMember[];
    loading: boolean;
}

export class TeamMembers extends React.Component<ITeamMembersProps, IteamMembersState> {
    constructor(props: ITeamMembersProps) {
        super(props);

        // This binding is necessary to make `this` work in the callback
        this.addMember = this.addMember.bind(this);
        this.removeMember = this.removeMember.bind(this);

        this.state = {
            members: props.members,
            loading: false
        };
    }

    addMember(e: any) {
        console.log("Adding member");
        this.setState({
            members: this.state.members.concat([{ name: "hopa", includeInReports: false }])
        }, () => {
            console.log("Added member");
        });
    }

    removeMember(member: ITeamMember, e: any) {
        console.log("Removing member " + member.name);
    }

    public render() {
        let nodes = this.state.members.map((member) => function (member: ITeamMember, thisArg: TeamMembers) {
            return (
                <div className="row">
                    <div className="col-sm-9">{member.name}</div>
                    <div className="col-sm-1">
                        <input type="button" className="btn btn-warning" value="remove" onClick={e => thisArg.removeMember(member, e)} />
                    </div>
                </div>
            );
        });

        return (
            <div className="panel panel-default section-block" >
                <div className="panel-heading"><h4>Members</h4></div>
                <div className="panel-body">
                    <div className="container-fluid">
                        {nodes}
                    </div>
                </div>
                <div className="panel-footer clearfix">
                    <div className="add-item-block">
                        <div className="input-group">
                            <span className="input-group-addon"><i className="glyphicon glyphicon-user"></i></span>
                            <div className="inline-inputs">
                                <input id="txtMemberName" type="text" className="form-control" name="txtMemberName" placeholder="Member name" />
                                <input type="button" className="btn-success" value="Add" onClick={this.addMember} />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}