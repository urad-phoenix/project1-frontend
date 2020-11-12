using System.Collections.Generic;
using System.IO;
using System.Linq;
using Phoenix.Playables.Markers;
using Regulus.Extension;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace Phoenix.Project1.Editors.Tools
{
    public class TimelineOutputExcelTool
    {                
        public static TimelineOutputData ConvertData(string key, TimelineAsset timelineAsset)
        {
            int frame = Mathf.FloorToInt((float)timelineAsset.duration * 30);           
                
            var timelineData = new TimelineOutputData();

            timelineData.Key = key;

            timelineData.TotalFrame = frame;
            
            var hitDatas = new List<TimelineHitData>();

            var tracks = timelineAsset.GetOutputTracks();

            foreach (var track in tracks)
            {
                var markers = track.GetMarkers();

                var hits = from marker in markers
                    where marker is PlayableEventMarker
                    select new TimelineHitData() {Key = key, Frame = Mathf.FloorToInt((float)marker.time * 30) } ;                
                
                hitDatas.AddRange(hits);
            }

            timelineData.HitDatas = hitDatas;

            return timelineData;
        }

        public static List<TimelineAsset> GetTimelineFiles(string path, string[] filterTypes)
        {   
            List<TimelineAsset> assets = new List<TimelineAsset>();

            var paths =GetAllFiles(path, filterTypes);

            for (int i = 0; i < paths.Count; ++i)
            {
                var asset = AssetDatabase.LoadAssetAtPath(paths[i], typeof(TimelineAsset)) as TimelineAsset;

                if (asset)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }

        public static List<string> GetAllFiles(string directoryPath, string[] filterTypes)
        {
            string[] files = Directory.GetFiles(directoryPath, "*",SearchOption.AllDirectories);
			
            List<string> paths = new List<string>();
            
            foreach (string file in files)
            {               
                if (file.Contains(".meta"))				
                    continue;                            
                
                if (filterTypes.Index(x => file.Contains(x)) > -1)                
                {                    
                    var assetPath = PathUtilities.AssetPath(file);
				
                    if (!string.IsNullOrEmpty(assetPath) && AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)))
                    {
                        paths.Add(assetPath);       
                    }
                }                
            }

            return paths;
        }              

        public string CheckFilePath(string path)
        {
            path = path.Replace("/", @"\");                        

            return path;
        }               
    }
}