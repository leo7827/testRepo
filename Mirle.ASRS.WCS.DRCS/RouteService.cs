using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mirle.Universal.Logger;

namespace Mirle.ASRS.WCS.DRCS
{
    public class RouteService : IDisposable
    {
        private class LogTrace
        {
            public string Message { get; }

            public LogTrace(string message, ILocation node)
            {
                Message = $"{message}|{node.DeviceID}-{node.LocationId}";
            }

            public LogTrace(string message, ILocation node, ILocation nextLocation)
            {
                Message = $"{message}|{node.DeviceID}-{node.LocationId}=>{nextLocation.DeviceID}-{nextLocation.LocationId}";
            }

            public LogTrace(string message, ILocation node, ILocation nextLocation, uint cost)
            {
                Message = $"{message}|{node.DeviceID}-{node.LocationId}=>{nextLocation.DeviceID}-{nextLocation.LocationId}|{cost}";
            }
        }

        private const string _LogFileName = "RouteService";
        private const uint _DefaultCost = 100;

        private readonly Dictionary<ILocation, List<ILocation>> _DevicePath = new Dictionary<ILocation, List<ILocation>>();
        private readonly Dictionary<ILocation, List<ILocation>> _Path = new Dictionary<ILocation, List<ILocation>>();
        private readonly Dictionary<ILocation, List<ILocation>> _DisablePath = new Dictionary<ILocation, List<ILocation>>();
        private readonly Dictionary<ILocation, Dictionary<ILocation, uint>> _PathCost = new Dictionary<ILocation, Dictionary<ILocation, uint>>();
        private readonly Dictionary<ILocation, Dictionary<ILocation, uint>> _DisablePathCost = new Dictionary<ILocation, Dictionary<ILocation, uint>>();
        private readonly Dictionary<ILocation, Dictionary<ILocation, uint>> _PathDefaultCost = new Dictionary<ILocation, Dictionary<ILocation, uint>>();

        private readonly Log _Log = new Log();
        private readonly object _LogLock = new object();

        public RouteService()
        {
        }

        public void AddDevicePath(ILocation node, ILocation nextLocation, uint cost)
        {
            try
            {
                if (_DevicePath.ContainsKey(node) == false)
                {
                    _Path.Add(node, new List<ILocation>());
                }
                if (_Path.ContainsKey(node) == false)
                {
                    _Path.Add(node, new List<ILocation>());
                }
                if (_DisablePath.ContainsKey(node) == false)
                {
                    _DisablePath.Add(node, new List<ILocation>());
                }
                if (_PathCost.ContainsKey(node) == false)
                {
                    _PathCost.Add(node, new Dictionary<ILocation, uint>());
                }
                if (_DisablePathCost.ContainsKey(node) == false)
                {
                    _DisablePathCost.Add(node, new Dictionary<ILocation, uint>());
                }
                if (_PathDefaultCost.ContainsKey(node) == false)
                {
                    _PathDefaultCost.Add(node, new Dictionary<ILocation, uint>());
                }

                _Path[node].Add(nextLocation);
                _PathCost[node][nextLocation] = _DefaultCost + cost;
                _PathDefaultCost[node][nextLocation] = _DefaultCost + cost;
                _DevicePath[node].Add(nextLocation);
                WriteLog(new LogTrace("Add Device Path", node, nextLocation));
            }
            catch (Exception ex)
            {
                int errorLine = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public void AddDevicePath(ILocation node, ILocation nextLocation)
        {
            AddDevicePath(node, nextLocation, 0);
        }

        public void AddPath(ILocation node, ILocation nextLocation, uint cost)
        {
            try
            {
                if (_Path.ContainsKey(node) == false)
                {
                    _Path.Add(node, new List<ILocation>());
                }
                if (_DisablePath.ContainsKey(node) == false)
                {
                    _DisablePath.Add(node, new List<ILocation>());
                }
                if (_PathCost.ContainsKey(node) == false)
                {
                    _PathCost.Add(node, new Dictionary<ILocation, uint>());
                }
                if (_DisablePathCost.ContainsKey(node) == false)
                {
                    _DisablePathCost.Add(node, new Dictionary<ILocation, uint>());
                }
                if (_PathDefaultCost.ContainsKey(node) == false)
                {
                    _PathDefaultCost.Add(node, new Dictionary<ILocation, uint>());
                }

                _Path[node].Add(nextLocation);
                _PathCost[node][nextLocation] = _DefaultCost + cost;
                _PathDefaultCost[node][nextLocation] = _DefaultCost + cost;

                WriteLog(new LogTrace("Add Path", node, nextLocation));
            }
            catch (Exception ex)
            {
                int errorLine = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public void AddPath(ILocation node, ILocation nextLocation)
        {
            AddPath(node, nextLocation, 0);
        }

        public void SetCost(ILocation node, ILocation nextLocation, uint cost)
        {
            try
            {
                if (_PathCost.ContainsKey(node) == false)
                {
                    _PathCost.Add(node, new Dictionary<ILocation, uint>());
                }
                if (_DisablePathCost.ContainsKey(node) == false)
                {
                    _DisablePathCost.Add(node, new Dictionary<ILocation, uint>());
                }
                if (_PathDefaultCost.ContainsKey(node) == false)
                {
                    _PathDefaultCost.Add(node, new Dictionary<ILocation, uint>());
                }

                _PathCost[node][nextLocation] = cost;
                _PathDefaultCost[node][nextLocation] = cost;
                WriteLog(new LogTrace("Set Cost", node, nextLocation, cost));
            }
            catch (Exception ex)
            {
                int errorLine = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public void SetCost(ILocation node, ILocation nextLocation)
        {
            SetCost(node, nextLocation, 0);
        }

        public void AddCost(ILocation node, ILocation nextLocation, uint cost)
        {
            try
            {
                if (_PathCost.TryGetValue(node, out var costList) == false)
                {
                    _PathCost.Add(node, new Dictionary<ILocation, uint>());
                    uint sum = _DefaultCost + cost;
                    _PathCost[node][nextLocation] = sum;
                }
                else
                {
                    if (costList.TryGetValue(nextLocation, out uint originalCost))
                    {
                        uint sum = originalCost + cost;
                        costList[nextLocation] = sum;
                    }
                    else
                    {
                        uint sum = _DefaultCost + cost;
                        costList.Add(nextLocation, sum);
                    }
                }
                WriteLog(new LogTrace("Add Cost", node, nextLocation, cost));
            }
            catch (Exception ex)
            {
                int errorLine = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public void EnalePath(ILocation node, ILocation nextLocation, bool enable)
        {
            try
            {
                if (enable)
                {
                    if (_DisablePath.TryGetValue(node, out var disableNextLocations))
                    {
                        if (disableNextLocations.Contains(nextLocation))
                        {
                            if (_Path.TryGetValue(node, out var enableNextLocations) == false)
                            {
                                _Path.Add(node, new List<ILocation>());
                            }

                            _DisablePathCost[node].TryGetValue(nextLocation, out uint cost);
                            _DisablePathCost[node].Remove(nextLocation);
                            _PathCost[node].Add(nextLocation, cost);
                            _Path[node].Add(nextLocation);
                            _DisablePath[node].Remove(nextLocation);
                        }
                        else
                        {
                            if (_Path.ContainsKey(node) == false)
                            {
                                //沒有路徑
                            }
                        }
                    }
                    else
                    {
                        if (_Path.ContainsKey(node) == false)
                        {
                            //沒有路徑
                        }
                    }
                    WriteLog(new LogTrace("Enable Path", node, nextLocation));
                }
                else
                {
                    if (_Path.TryGetValue(node, out var enableNextLocations))
                    {
                        if (enableNextLocations.Contains(nextLocation))
                        {
                            if (_DisablePath.TryGetValue(node, out var disableNextLocations) == false)
                            {
                                _DisablePath.Add(node, new List<ILocation>());
                            }
                            if (_DisablePathCost.ContainsKey(node) == false)
                            {
                                _DisablePathCost.Add(node, new Dictionary<ILocation, uint>());
                            }

                            _PathCost[node].TryGetValue(nextLocation, out uint cost);
                            _DisablePathCost[node].Add(nextLocation, cost);
                            _DisablePath[node].Add(nextLocation);
                            _Path[node].Remove(nextLocation);
                            _PathCost[node].Remove(nextLocation);
                        }
                        else
                        {
                            if (_DisablePath.ContainsKey(node) == false)
                            {
                                //沒有路徑
                            }
                        }
                    }
                    else
                    {
                        if (_DisablePath.ContainsKey(node) == false)
                        {
                            //沒有路徑
                        }
                    }
                    WriteLog(new LogTrace("Disable Path", node, nextLocation));
                }
            }
            catch (Exception ex)
            {
                int errorLine = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public void EnalePath(ILocation node, bool enable)
        {
            try
            {
                if (enable)
                {
                    if (_DisablePath.TryGetValue(node, out var disableNextLocations))
                    {
                        if (_Path.TryGetValue(node, out var enableNextLocations) == false)
                        {
                            _Path.Add(node, new List<ILocation>());
                        }

                        foreach (var nextLocation in enableNextLocations)
                        {
                            _DisablePathCost[node].TryGetValue(nextLocation, out uint cost);
                            _DisablePathCost[node].Remove(nextLocation);
                            _PathCost[node][nextLocation] = cost;
                        }

                        _Path[node].AddRange(disableNextLocations);
                        _DisablePath.Remove(node);
                    }
                    else
                    {
                        if (_Path.ContainsKey(node) == false)
                        {
                            //沒有路徑
                        }
                    }
                    WriteLog(new LogTrace("Enable Node", node, node));
                }
                else
                {
                    if (_Path.TryGetValue(node, out var enableNextLocations))
                    {
                        if (_DisablePath.TryGetValue(node, out var disableNextLocations) == false)
                        {
                            _DisablePath.Add(node, new List<ILocation>());
                        }
                        if (_DisablePathCost.ContainsKey(node) == false)
                        {
                            _DisablePathCost.Add(node, new Dictionary<ILocation, uint>());
                        }

                        _PathCost.TryGetValue(node, out var enableNextLocationsCost);
                        _DisablePathCost[node] = enableNextLocationsCost;
                        _DisablePath[node].AddRange(disableNextLocations);
                        _Path.Remove(node);
                        _PathCost.Remove(node);
                    }
                    else
                    {
                        if (_DisablePath.ContainsKey(node) == false)
                        {
                            //沒有路徑
                        }
                    }
                    WriteLog(new LogTrace("Disable Node", node, node));
                }
            }
            catch (Exception ex)
            {
                int errorLine = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }
        }

        public void ResetCost()
        {
            _PathCost.Clear();
            foreach (var node in _PathDefaultCost)
            {
                foreach (var nextNode in node.Value)
                {
                    _PathCost.Add(node.Key, new Dictionary<ILocation, uint>());
                    _PathCost[node.Key][nextNode.Key] = nextNode.Value;
                }
            }
        }

        public void ClearPath()
        {
            _Path.Clear();
            _PathCost.Clear();
        }

        public IEnumerable<ILocation> GetPath(ILocation startLocation, ILocation endLocation)
        {
            var path = new List<ILocation>();
            try
            {
                var prior = Dijkstra(startLocation);
                AddPathPoint(endLocation, startLocation);

                void AddPathPoint(ILocation end, ILocation start)
                {
                    if (end.Equals(start))
                    {
                        return;
                    }

                    if (prior.TryGetValue(end, out var next))
                    {
                        if (typeof(ILocation).IsValueType && next.Equals(default(ILocation)) || next == null)
                        {
                            return;
                        }

                        path.Add(end);
                        if (next.Equals(start))
                        {
                            path.Add(start);
                            path.Reverse();
                            return;
                        }
                        AddPathPoint(next, start);
                    }
                }
            }
            catch (Exception ex)
            {
                int errorLine = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
                var cmet = MethodBase.GetCurrentMethod();
                clsWriLog.Log.subWriteExLog(cmet.DeclaringType.FullName + "." + cmet.Name, errorLine.ToString() + ":" + ex.Message);
            }

            return path;
        }

        public IEnumerable<KeyValuePair<ILocation, ILocation>> GetAvailablePath()
        {
            var keyValuePairs = new List<KeyValuePair<ILocation, ILocation>>();
            foreach (var node1 in _Path.Keys)
            {
                foreach (var node2 in _Path[node1])
                {
                    keyValuePairs.Add(new KeyValuePair<ILocation, ILocation>(node1, node2));
                }
            }
            return keyValuePairs;
        }

        public IEnumerable<KeyValuePair<ILocation, ILocation>> GetUnavailablePath()
        {
            var keyValuePairs = new List<KeyValuePair<ILocation, ILocation>>();
            foreach (var node1 in _DisablePath.Keys)
            {
                foreach (var node2 in _DisablePath[node1])
                {
                    keyValuePairs.Add(new KeyValuePair<ILocation, ILocation>(node1, node2));
                }
            }
            return keyValuePairs;
        }

        public IEnumerable<KeyValuePair<ILocation, ILocation>> GetAllPath()
        {
            var keyValuePairs = new List<KeyValuePair<ILocation, ILocation>>();
            foreach (var node1 in _Path.Keys)
            {
                foreach (var node2 in _Path[node1])
                {
                    keyValuePairs.Add(new KeyValuePair<ILocation, ILocation>(node1, node2));
                }
            }
            foreach (var node1 in _DisablePath.Keys)
            {
                foreach (var node2 in _DisablePath[node1])
                {
                    keyValuePairs.Add(new KeyValuePair<ILocation, ILocation>(node1, node2));
                }
            }
            return keyValuePairs;
        }

        public bool IsDevicePath(ILocation node, ILocation nextLocation)
        {
            if (_DevicePath.ContainsKey(node))
            {
                return _DevicePath[node].Contains(nextLocation);
            }
            else
            {
                return false;
            }
        }

        public bool IsPath(ILocation node, ILocation nextLocation)
        {
            if (_Path.ContainsKey(node))
            {
                return _Path[node].Contains(nextLocation);
            }
            else
            {
                return false;
            }
        }

        public string Print()
        {
            var str = new StringBuilder();
            str.AppendLine($"Locations:");
            foreach (var keyValuePairMap in _Path)
            {
                str.AppendLine($"{keyValuePairMap.Key.DeviceID}-{keyValuePairMap.Key.LocationId} => {string.Join(", ", keyValuePairMap.Value.Select(x => $"{x.DeviceID}-{x.LocationId}"))}");
            }
            str.AppendLine();
            str.AppendLine($"Cost:");
            str.AppendLine($"{string.Empty.PadRight(10, ' ')}\t\t{string.Join("\t\t", _Path.Keys.Select(x => $"{x.DeviceID}-{x.LocationId}".PadRight(10, ' ')))}");
            foreach (var node in _Path.Keys)
            {
                str.Append($"{node.DeviceID}-{node.LocationId}".PadRight(10, ' '));
                if (_PathCost.TryGetValue(node, out var oLocationCosts))
                {
                    foreach (var oLocation in _Path.Keys)
                    {
                        if (oLocationCosts.TryGetValue(oLocation, out uint cost))
                        {
                            str.Append($"\t\t{cost.ToString().PadRight(10, ' ')}");
                        }
                        else
                        {
                            str.Append($"\t\t{"0".PadRight(10, ' ')}");
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < _Path.Keys.Count; i++)
                    {
                        str.Append($"\t0");
                    }
                }
                str.AppendLine();
            }
            return str.ToString();
        }

        private void WriteLog(LogTrace logTrace)
        {
            try
            {
                lock (_LogLock)
                {
                    _Log.WriteLogFile(_LogFileName, logTrace.Message);
                }
            }
            catch (Exception ex)
            { Debug.WriteLine($"{MethodBase.GetCurrentMethod()}: {ex.Message}\n{ex.StackTrace}"); }
        }

        private Dictionary<ILocation, ILocation> Dijkstra(ILocation start)
        {
            //Create Map
            var map = new Dictionary<ILocation, List<ILocation>>();
            foreach (var keyValuePairLocation in _Path)
            {
                try
                {
                    var list = new List<ILocation>();
                    map.Add(keyValuePairLocation.Key, list);
                    foreach (var oLocation in keyValuePairLocation.Value)
                    {
                        try
                        {
                            if (_Path.ContainsKey(oLocation))
                            {
                                list.Add(oLocation);
                            }
                            else
                            {
                                if (!map.ContainsKey(oLocation))
                                {
                                    map.Add(oLocation, new List<ILocation>());
                                }
                                if (!_PathCost.ContainsKey(oLocation))
                                {
                                    _PathCost.Add(oLocation, new Dictionary<ILocation, uint>());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}"); 
                }
            }

            //point, dist
            var dist = new Dictionary<ILocation, uint>();
            foreach (var p in map)
            {
                dist[p.Key] = 1;
            }

            //point, prior
            var prior = new Dictionary<ILocation, ILocation>();
            foreach (var p in map)
            {
                prior[p.Key] = default(ILocation);
            }

            //point, decided
            var decided = new Dictionary<ILocation, bool>();
            foreach (var p in map)
            {
                decided[p.Key] = false;
            }

            foreach (var point in map)
            {
                var key = point.Key;
                if (_PathCost[start].TryGetValue(key, out uint value))
                {
                    dist[key] = value;
                }
                else
                {
                    dist[key] = uint.MaxValue;
                }

                {
                }
                prior[key] = start;
                decided[key] = false;
            }

            try
            {
                //Dijkstra
                decided[start] = true;
                var Vx = default(ILocation);
                foreach (var point in map)
                {
                    find_min(ref Vx);
                    if (typeof(ILocation).IsValueType && Vx.Equals(default(ILocation)) || Vx == null)
                    {
                        foreach (var d in decided)
                        {
                            if (d.Value == false)
                            {
                                prior[d.Key] = default(ILocation);
                            }
                        }
                        return prior;
                    }
                    decided[Vx] = true;
                    foreach (var w in _PathCost[Vx])
                    {
                        var key = w.Key;
                        if (_PathCost[Vx].TryGetValue(key, out uint couldCost) == false)
                        {
                            couldCost = uint.MaxValue;
                        }

                        if (w.Value < uint.MaxValue && !decided[key] && dist[key] > dist[Vx] + couldCost)
                        {
                            dist[key] = dist[Vx] + couldCost;
                            prior[key] = Vx;
                        }
                    }
                }
            }
            catch (Exception ex)
            { Debug.WriteLine($"{ex.Message}\n{ex.StackTrace}"); }

            foreach (var d in decided)
            {
                if (d.Value == false)
                {
                    prior[d.Key] = default(ILocation);
                }
            }
            return prior;

            void find_min(ref ILocation vx)
            {
                var low = default(ILocation);
                uint lowest = uint.MaxValue;
                foreach (var point in map.Keys)
                {
                    if (decided[point] == false && dist[point] < lowest)
                    {
                        lowest = dist[point];
                        low = point;
                    }
                }
                vx = low;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                _Log?.Dispose();

                disposedValue = true;
            }
        }

        ~RouteService()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
