import 'isomorphic-fetch';
import * as React from 'react';

import { ITeamInfo } from './dataTransfer/ITeamInfo';
import { ITeamMember } from './dataTransfer/ITeamMember';

import { TeamMembers } from './TeamMembers';
import { TeamRepos } from './TeamRepos';

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

        this.state = { members: [{ name: "mkArtak", includeInReports: false }], repositories: ["aspnet/Razor"] };
    }

    componentDidMount() {
        /*fetch('api/Teams/' + this.props.teamName)
            .then(response => response.json() as Promise<TeamInfo>)
            .then(data => {
                this.setState({ members: data.teamMembers, loading: false });
            });*/
        // this.setState({
        //     members: [{ name: "mkArtak", includeInReports: false }],
        //     repositories: ['aspnet/test']
        // });
    }

    componentWillUnmount() {

    }

    render() {
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