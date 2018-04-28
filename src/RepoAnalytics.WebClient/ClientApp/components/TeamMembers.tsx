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
    newMemberName: string;
}

export class TeamMembers extends React.Component<ITeamMembersProps, IteamMembersState> {
    constructor(props: ITeamMembersProps) {
        super(props);

        // This binding is necessary to make `this` work in the callback
        this.addMember = this.addMember.bind(this);
        this.removeMember = this.removeMember.bind(this);
        this.onNewMemberNameChanged = this.onNewMemberNameChanged.bind(this);
        
        this.state = {
            members: props.members,
            loading: false,
            newMemberName: ""
        };
    }

    addMember(e: any) {
        console.log("Adding member " + this.state.newMemberName);
        this.setState({
            // Call the controller ADD method here passing in the member alias here
            members: this.state.members.concat([{ name: this.state.newMemberName, includeInReports: false }])
        }, () => {
            console.log("Added member");
        });
    }

    removeMember(member: ITeamMember, e: any) {
        console.log("Removing member " + member.name);
    }

    onNewMemberNameChanged(event: any) {
        this.setState({ newMemberName: event.target.value });
    }

    public render() {
        let nodes = this.state.members.map(function (member: ITeamMember) {
            return (
                <div className="row" key={member.name}>
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
                        {nodes}
                    </div>
                </div>
                <div className="panel-footer clearfix">
                    <div className="add-item-block">
                        <div className="input-group">
                            <span className="input-group-addon"><i className="glyphicon glyphicon-user"></i></span>
                            <div className="inline-inputs">
                                <input type="text" className="form-control" placeholder="Member name" value={this.state.newMemberName} onChange={this.onNewMemberNameChanged} />
                                <input type="button" className="btn-success" value="Add" onClick={this.addMember} />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}