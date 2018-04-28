import * as React from 'react';
import 'isomorphic-fetch';
import { TeamMember, TeamInfo } from './Interfaces';

interface ITeamReposProps {
    teamName: string;
    repos: string[];
}

interface IteamReposState {
    repos: string[];
    loading: boolean;
}

export class TeamRepos extends React.Component<ITeamReposProps, IteamReposState> {
    constructor(props: ITeamReposProps) {
        super(props);

        this.state = {
            repos: props.repos,
            loading: false
        };
    }

    public render() {
        let nodes = this.state.repos.map(function (repo: string) {
            return (
                <div className="row">
                    <div className="col-sm-9">{repo}</div>
                    <div className="col-sm-1">
                        <input type="button" className="btn btn-warning" value="remove" />
                    </div>
                </div>
            );
        });

        return (
            <div className="panel panel-default section-block" >
                <div className="panel-heading"><h4>Repositories</h4></div>
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
                                <input id="txtRepoName" type="text" className="form-control" name="txtRepoName" placeholder="Member name" />
                                <input type="button" id="btnAddRepo" name="btnAddRepo" className="btn-success" value="Add" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}