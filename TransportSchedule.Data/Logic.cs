using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportSchedule.Data {
    public class Logic {
        public event Message NewMessage;

        const string FileName = "data.txt";

        // Helper method to skip comment lines in data.txt
        private string ReadNonCommentLine(StreamReader sr) {
            string line = sr.ReadLine();
            while (line.StartsWith("#"))
                line = sr.ReadLine();
            return line;
        }

        private void ReadData(out List<Route> routes, out List<Station> stations) {
            routes = new List<Route>();
            stations = new List<Station>();

            using (var sr = new StreamReader(FileName)) {
                int count = int.Parse(ReadNonCommentLine(sr));
                string line;
                string[] parts;

                for (int i = 0; i < count; i++) {
                    line = ReadNonCommentLine(sr);
                    parts = line.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2) {
                        var station = new Station
                        {
                            Id = int.Parse(parts[0]),
                            Name = parts[1]
                        };
                        stations.Add(station);
                    }
                }

                count = int.Parse(ReadNonCommentLine(sr));
                for (int i = 0; i < count; i++) {
                    line = ReadNonCommentLine(sr);
                    parts = line.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 4) {
                        var route = new Route(parts[0], int.Parse(parts[1]),
                            int.Parse(parts[2]), int.Parse(parts[3]));
                        // Stations on route
                        line = ReadNonCommentLine(sr);
                        parts = line.Split(';');
                        foreach (var part in parts) {
                            int stationId = int.Parse(part);
                            var station = stations.FirstOrDefault(st => st.Id == stationId);
                            if (station == null)
                                throw new ArgumentException("Incorrect file format. Station not found");
                            route.Stations.Add(new RouteStation
                            {
                                Station = station
                            });
                        }

                        // Interval between stations
                        line = ReadNonCommentLine(sr);
                        parts = line.Split(';');
                        if (parts.Length != route.Stations.Count - 1)
                            throw new ArgumentException("Incorrect file format. Number of intervals does not match number of stations");
                        int totalDistance = 0;
                        route.Stations[0].TimeFromOrigin = 0;
                        for (int k = 0; k < parts.Length; k++) {
                            totalDistance += int.Parse(parts[k]);
                            route.Stations[k + 1].TimeFromOrigin = totalDistance;
                        }
                        // Going back to fill TimeFromDest
                        for (int k = parts.Length - 1; k >= 0; k--)
                            route.Stations[k].TimeFromDest = totalDistance - route.Stations[k].TimeFromOrigin;
                        routes.Add(route);
                    }
                }
            }
        }


        public List<Station> GetStations()
        {
            List<Route> routes;
            List<Station> stations;

            try
            {
                ReadData(out routes, out stations);
                return stations;
            }
            catch (Exception ex)
            {
                NewMessage?.Invoke("Error reading file: " + ex.Message, "Ошибка");
                return null;
            }
        }

        public List<ScheduleItem> GetSchedule(Station _station) {
            List<Route> routes;
            List<Station> stations;

            try {
                ReadData(out routes, out stations);
            }
            catch (Exception ex) {
                NewMessage?.Invoke("Error reading file: " + ex.Message, "Ошибка");
                return null;
            }

            while (true) {
                var station = stations.FirstOrDefault(st => st.Id == _station.Id);
                if (station == null)
                    return null;
                else
                {
                    List<ScheduleItem> result = new List<ScheduleItem>();

                    // Call to DateTime.Now only once to prevent different readings on different loop iterations
                    // Here manual time can also be set to test the algorithm
                    DateTime currentDt = DateTime.Now;

                    foreach (var route in routes)
                    {

                        var routeStation = route.Stations
                            .FirstOrDefault(st => st.Station == station);

                        if (routeStation != null)
                        {

                            if (routeStation != route.Stations.Last())
                            {
                                int left = route.TimeToNextDepartureFromOrigin(routeStation, currentDt);
                                result.Add(new ScheduleItem
                                {
                                    RouteName = route.Name,
                                    Destination = route.Stations.Last().Station,
                                    MinutesLeft = left
                                });
                            }
                            if (routeStation != route.Stations.First())
                            {
                                int left = route.TimeToNextDepartureFromDest(routeStation, currentDt);
                                result.Add(new ScheduleItem
                                {
                                    RouteName = route.Name,
                                    Destination = route.Stations.First().Station,
                                    MinutesLeft = left
                                });
                            }
                        }
                    }
                    return result;
                }
            }
        }
    }
}
