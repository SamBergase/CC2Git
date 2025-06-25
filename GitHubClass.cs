using System;
using Octokit;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReadRepo
{
	public class GitHub
	{
		public GitHub()
		{
        }

		public async Task<List<GitHubCommit>> getAllCommits(string username, string repo)
        {
			var github = new GitHubClient(new ProductHeaderValue("MyAmazingApp"));
			var repository = await github.Repository.Commit.Get(username, repo);
			var commits = await github.Repository.Commit.GetAll(repository.id);

			List<GitHubCommit> commitList = new List<GitHubCommit>();

			foreach(GitHubCommit commit in commits)
            {
				//var commitDetails = gitub.Repository.Commit.Get(commit.Sha);
				//var files = commitDetails.Files;
				commitList.Add(commit);
            }
			return commitList;
        }
    }
}
