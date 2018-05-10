import "isomorphic-fetch";
import * as React from "react";

import { ITeamInfo } from "./dataTransfer/ITeamInfo";
import { ITeamMember } from "./dataTransfer/ITeamMember";

import { TeamMembers } from "./TeamMembers";
import { TeamRepos } from "./TeamRepos";

interface ITeamEditorProps {
    teamName: string;
}

interface ITeamEditorState {
    members: ITeamMember[];
    repositories: string[];
    defaultMilestones: string;
}

export class TeamEditor extends React.Component<ITeamEditorProps, ITeamEditorState> {
    constructor(props: ITeamEditorProps) {
        super(props);

        this.state = {
            members: [],
            repositories: [],
            defaultMilestones: ""
        };
    }

    componentDidMount() {
        fetch("api/Teams/" + this.props.teamName)
            .then(response => response.json() as Promise<ITeamInfo>)
            .then(data => {
                this.setState({
                    members: data.teamMembers,
                    repositories: data.repositories,
                    defaultMilestones: data.defaultMilestonesToTrack
                });
            });
    }

    private handleDefaultMilestonesChange(event: any) {
        let value: string = event.target.value;
        this.setState({
            defaultMilestones: value
        });
    }

    render() {
        console.log("Rendering teamEditor with members " + this.state.members.length);
        return (
            <div>
                <div className="label-data-block">
                    <label htmlFor="defaultMilestones" className="field-label">Default milestone:</label>
                    <input type="text" id="defaultMilestones" value={this.state.defaultMilestones} onChange={this.handleDefaultMilestonesChange} />
                    <button className="btn btn-default"><span className="glyphicons glyphicons-floppy-disk"></span> Save</button>
                </div>
                <div className="clearfix">
                    <div className="section-block">
                        <TeamMembers teamName={this.props.teamName} members={this.state.members} />
                    </div>
                    <div className="section-block">
                        <TeamRepos teamName={this.props.teamName} repositories={this.state.repositories} />
                    </div>
                </div>
            </div>
        );
    }
}