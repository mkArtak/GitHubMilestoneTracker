import * as React from "react";
import "isomorphic-fetch";
import { ITeamMember } from "./dataTransfer/ITeamMember";
import { ITeamInfo } from "./dataTransfer/ITeamInfo";

interface ITeamMembersProps {
    teamName: string;
    members: ITeamMember[];
}

interface ITeamMembersState {
    members: ITeamMember[];
    loading: boolean;
    newMemberName: string;
    canAddCurrentMember: boolean;
}

export class TeamMembers extends React.Component<ITeamMembersProps, ITeamMembersState> {
    constructor(props: ITeamMembersProps) {
        super(props);

        // this binding is necessary to make `this` work in the callback scenarios
        this.addMember = this.addMember.bind(this);
        this.removeMember = this.removeMember.bind(this);
        this.onNewMemberNameChanged = this.onNewMemberNameChanged.bind(this);

        this.state = {
            members: props.members,
            loading: false,
            newMemberName: "",
            canAddCurrentMember: false
        };
    }

    componentWillReceiveProps(newProps: ITeamMembersProps): void {
        this.setState({
            members: newProps.members,
        });
    }

    private addMember(e: any) {
        console.log("Adding member " + this.state.newMemberName);
        if (this.state.newMemberName === "" || this.memberExists(this.state.newMemberName)) {
            return;
        }

        fetch("api/Teams/" + this.props.teamName + "/members/" + this.state.newMemberName,
            {
                method: "POST",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                },
            }).then(() =>
                this.setState({
                    members: this.state.members.concat([{
                        name: this.state.newMemberName, includeInReports: false
                    }]),
                    newMemberName: "",
                    canAddCurrentMember: false
                }));
    }

    public updateMembers(members: ITeamMember[]) {
        this.setState({ members: members });
    }

    private removeMember(member: ITeamMember, e: any) {
        var memberIndex = this.getMemberIndex(member.name);
        if (memberIndex === -1) {
            return;
        }

        fetch("api/Teams/" + this.props.teamName + "/members/" + member.name,
            {
                method: "DELETE",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                }
            })
            .then(response =>
                this.setState({
                    members: this.state.members.filter((_, i) => i !== memberIndex)
                }, () => {
                    console.log("Removed member " + member.name);
                }));
    }

    private onNewMemberNameChanged(event: any) {
        let newName = event.target.value;
        this.setState({
            newMemberName: newName,
            canAddCurrentMember: !this.memberExists(newName)
        });
    }

    private memberExists(memberName: string) {
        return this.getMemberIndex(memberName) !== -1;
    }

    private getMemberIndex(memberName: string) {
        for (var i = 0; i < this.state.members.length; i++) {
            if (this.state.members[i].name === memberName) {
                return i;
            }
        }

        return -1;
    }

    public render() {
        const _members: ITeamMember[] = this.state.members;
        let nodes: JSX.Element[] = _members.map(member => {
            return (
                <div className="row" key={member.name}>
                    <div className="col-sm-9">{member.name}</div>
                    <div className="col-sm-1">
                        <input type="button" className="btn btn-warning" value="remove" onClick={(e) => this.removeMember(member, e)} />
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
                                <input type="button" disabled={!this.state.canAddCurrentMember} className="btn-success" value="Add" onClick={this.addMember} />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}