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
}

export class TeamEditor extends React.Component<ITeamEditorProps, ITeamEditorState> {
    constructor(props: ITeamEditorProps) {
        super(props);

        this.state = {
            members: [],
            repositories: []
        };
    }

    /*componentWillUnmount() {

    }*/

    render() {
        console.log("Rendering teamEditor with members " + this.state.members.length);
        return (
            <div className="clearfix">
                <div className="section-block">
                    <TeamMembers teamName={this.props.teamName} members={this.state.members} />
                </div>
                <div className="clearfix">
                    <TeamRepos teamName={this.props.teamName} repositories={this.state.repositories} />
                </div>
            </div>
        );
    }
}