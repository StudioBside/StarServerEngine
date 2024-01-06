namespace Cs.Perforce;

using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using Cs.Logging;
using global::Perforce.P4;

public sealed class PerforceController : IDisposable
{
    private readonly Repository p4Repository;

    public PerforceController(P4ConnectionInfo info)
    {
        var server = new Server(new ServerAddress(info.Uri));
        this.p4Repository = new Repository(server);

        var connection = this.p4Repository.Connection;
        connection.UserName = info.Id;
        connection.Client = new Client { Name = info.Workspace };

        bool connected = connection.Connect(null);
        if (connected)
        {
            string password = info.Pw.DecodeBase64();
            try
            {
                Credential credential = this.PerforceCon.Login(password);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                this.PerforceCon.Disconnect();
            }
        }
    }

    private Connection PerforceCon => this.p4Repository.Connection;

    public bool TryGetHeadRevision(string depotPath, out int revision)
    {
        try
        {
            if (!this.PerforceCon.Connect(null))
            {
                revision = 0;
                return false;
            }

            var opts = new ChangesCmdOptions(
                ChangesCmdFlags.None,
                clientName: null,
                maxItems: 1,
                ChangeListStatus.Submitted,
                userName: null);
            var spec = new FileSpec(new DepotPath(depotPath), new HeadRevision());

            IList<Changelist> changes = this.p4Repository.GetChangelists(opts, spec) ?? new List<Changelist>();
            if (changes.Count == 0)
            {
                revision = 0;
                return false;
            }

            revision = changes[0].Id;
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            revision = 0;
            return false;
        }
    }

    public IList<Changelist>? GetChangeList(string depotPath, int lastRevision)
    {
        if (!this.PerforceCon.Connect(null))
        {
            return null;
        }

        try
        {
            var opts = new ChangesCmdOptions(
                ChangesCmdFlags.FullDescription,
                clientName: null,
                maxItems: 0,
                ChangeListStatus.Submitted,
                userName: null);
            var range = new VersionRange(new ChangelistIdVersion(lastRevision + 1), VersionSpec.Head);
            var spec = new FileSpec(new DepotPath(depotPath), range);

            return this.p4Repository.GetChangelists(opts, spec);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            this.PerforceCon.Disconnect();
            return null;
        }
    }

    public IList<Changelist>? GetChangeList(string depotPath, int lastRevision, Func<Changelist, bool> condition)
    {
        if (!this.PerforceCon.Connect(null))
        {
            return null;
        }

        IList<Changelist>? changes;
        try
        {
            var opts = new ChangesCmdOptions(
                ChangesCmdFlags.FullDescription,
                clientName: null,
                maxItems: 0,
                ChangeListStatus.Submitted,
                userName: null);
            var range = new VersionRange(new ChangelistIdVersion(lastRevision + 1), VersionSpec.Head);
            var spec = new FileSpec(new DepotPath(depotPath), range);

            changes = this.p4Repository.GetChangelists(opts, spec);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            this.PerforceCon.Disconnect();
            return null;
        }

        if (changes == null)
        {
            return null;
        }

        return changes.Where(condition).ToList();
    }

    public List<string> GetChangeFiles(int revision)
    {
        var command = new P4Command(this.p4Repository, "describe", false);
        var opts = new Options
        {
            ["-s"] = null,
            [revision.ToString()] = null,
        };

        var results = command.Run(opts);
        return results.InfoOutput.Select(e => e.Message).ToList();
    }

    public void Dispose()
    {
        this.p4Repository.Dispose();
    }
}
