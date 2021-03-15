using System;
using System.Collections.Generic;

namespace ZEDAssignment.APIResponse
{
    [Serializable]
    public class DongMeta
    {
        public int bd_id;
        public string 동;
        public int 지면높이;
    }

    [Serializable]
    public class RoomType
    {
        public List<string> coordinatesBase64s;
        public RoomTypeMeta meta;
    }

    [Serializable]
    public class RoomTypeMeta
    {
        public int 룸타입id;
    }

    [Serializable]
    public class DongData
    {
        public List<RoomType> roomtypes;
        public DongMeta meta;
    }

    [Serializable]
    public class Root
    {
        public bool success;
        public int code;
        public List<DongData> data;
    }
}