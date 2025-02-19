﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BruTile.Web;

namespace BruTile.Wmts
{
    using BruTile.Wmts.Generated;

    public class WmtsRequest : IRequest
    {
        public const string XTag = "{TileCol}";
        public const string YTag = "{TileRow}";
        public const string ZTag = "{TileMatrix}";
        public const string TileMatrixSetTag = "{TileMatrixSet}";
        public const string TimeTag = "{Time}";
        public const string StyleTag = "{Style}";

        private readonly List<ResourceUrl> _resourceUrls;
        private int _resourceUrlCounter;
        private readonly object _syncLock = new object();
        private readonly IDictionary<int, string> _levelToIdentifier;

        public WmtsRequest(IEnumerable<ResourceUrl> resourceUrls, IDictionary<int, string> levelToIdentifier)
        //public WmtsRequest(IEnumerable<ResourceUrl> resourceUrls, IDictionary<int, string> levelToIdentifier, string[] dimensionValues)
        {
            _resourceUrls = resourceUrls.ToList();
            _levelToIdentifier = levelToIdentifier;
        }

        public Uri GetUri(TileInfo info)
        {
            var urlFormatter = GetNextServerNode();
            var stringBuilder = new StringBuilder(urlFormatter.Template);

            // For wmts we need to map the level int to an identifier of type string.
            var identifier = _levelToIdentifier[info.Index.Level];
            var time = info.DimensionSettings.FirstOrDefault(setting => setting.Key.Equals("Time")).Value;
            stringBuilder.Replace(XTag, info.Index.Col.ToString(CultureInfo.InvariantCulture));
            stringBuilder.Replace(YTag, info.Index.Row.ToString(CultureInfo.InvariantCulture));
            stringBuilder.Replace(ZTag, identifier);
            stringBuilder.Replace(TimeTag, time);

            return new Uri(stringBuilder.ToString());
        }

        private ResourceUrl GetNextServerNode()
        {
            lock (_syncLock)
            {
                var serverNode = _resourceUrls[_resourceUrlCounter++];
                if (_resourceUrlCounter >= _resourceUrls.Count) _resourceUrlCounter = 0;
                return serverNode;
            }
        }
    }
}
