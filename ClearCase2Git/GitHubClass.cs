using System;
using Octokit;
using System.Threading.Tasks;
using System.Collections.Generic;
using LibGit2Sharp;

namespace ReadRepo
{
	public class GitHub
	{
        #region public parameters
        public List<string> setTags;
		//public IReadOnlyList<GitHubCommit> setGHC;
		//public SystemThreading.Tasks.Task<System.Collections.Generic.IReadOnlyList<Octokit.GitHubCommit>> setGHC;
		public System.Collections.Generic.IReadOnlyList<Octokit.GitHubCommit> setGHC;
		bool currRepoIsBare = false;
		bool currRepoIsHeadDetached = false;
		bool currRepoIsHeadUnborn = false;
		bool currRepoIsShallow = false;
		string currRepoMessage = "";
		string currRepoGitPath = "";
		string currRepoWorkDir = "";
		#endregion

		public GitHub()
		{
        }

		public string GetStatus(string localRepoPath)
        {
			string retVal = "";
			using (var repo = new LibGit2Sharp.Repository(@localRepoPath))
            {
				RepositoryInformation repInfo = repo.Info;

				int currHash = repInfo.GetHashCode();
				currRepoIsBare = repInfo.IsBare;
				currRepoIsHeadDetached = repInfo.IsHeadDetached;
				currRepoIsHeadUnborn = repInfo.IsHeadUnborn;
				currRepoIsShallow = repInfo.IsShallow;
				currRepoMessage = repInfo.Message;
				currRepoGitPath = repInfo.Path;
				currRepoWorkDir = repInfo.WorkingDirectory;
				//RepositoryInformation.
				//RepositoryContentInfo.
            }
			return retVal;
        }

		//public Task<List<string>> getAllCommits(string product, string username, string inurl)
		public void getAllCommits(string username, string inurl, string product, string repo)
		{
			// inurl = "https://uxc005.corp.saab.se/gitlab/
			// product = product category
			// repo = project name
			//using (var workRepo = new Octokit.Repository(repo))
            //{
			//	var commits = repo.Commits.QueryBy(new CheckRunCompletedAtFilter { Since = repo.Refs });
            //}
		}
	}
}
