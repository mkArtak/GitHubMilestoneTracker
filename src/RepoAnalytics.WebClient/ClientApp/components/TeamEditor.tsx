import 'isomorphic-fetch';
import * as React from 'react';
import { TeamMembers } from './TeamMembers';
import { TeamRepos } from './TeamRepos';
import { TeamInfo, TeamMember } from './Interfaces';

interface ITeamEditorProps {
    teamName: string;
}

interface ITeamEditorState {
    members: TeamMember[];
    repositories: string[];
}

export class TeamEditor extends React.Component<ITeamEditorProps, ITeamEditorState> {
    constructor(props: ITeamEditorProps) {
        super(props);

        this.state = { members: [], repositories: [] };
    }

    componentDidMount() {
        /*fetch('api/Teams/' + this.props.teamName)
            .then(response => response.json() as Promise<TeamInfo>)
            .then(data => {
                this.setState({ members: data.teamMembers, loading: false });
            });*/
        this.setState({
            members: [{ name: "mkArtak", includeInReports: false }],
            repositories: ['aspnet/test']
        });
    }

    componentWillUnmount() {

    }

    render() {
        return (
            <div>
                <div className="section-block">
                    <TeamMembers teamName={this.props.teamName} members={this.state.members} />
                    <TeamRepos teamName={this.props.teamName} repos={this.state.repositories} />
                </div>
            </div>
        );
    }
}