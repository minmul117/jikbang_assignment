using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZEDAssignment.APIResponse;
using ZEDAssignment.ResponseParser;

namespace ZEDAssignment.Core
{
    public class Architect : MonoBehaviour
    {
        public TextAsset RepsonseJson;
        public MeshGenerator MeshGenerator;
        
        List<Vector3> testVertices;

        void Start()
        {
            testVertices = new List<Vector3>();
            SendRequest();
        }

        public void SendRequest()
        {
            // NOTE(minmul117): API 리퀘스트를 던지고 응답을 받는 부분.
            // 원래대로라면 응답 던지고 / 받는 부분 둘 다 이 클래스에서 분리되어 있어야 하나...
            // 실제로 응답을 가져오지는 않으니 지금은 패스한다.
            var Parser = new ResponseParser.Parser();
            var Response = Parser.ParseResponse(RepsonseJson.text);
            
            if (Response.success && Response.code == 200)
            {
                DrawVertices(Response);
            }
        }

        private void DrawVertices(Root Response)
        {
            foreach (DongData dongData in Response.data)
            {
                var dongName = dongData.meta.동;
                // 한 동의 verticese는 동 데이터 안에 있다.
                foreach (RoomType roomType in dongData.roomtypes)
                {
                    ProcessVertices(roomType);
                }
            }
            
            MeshGenerator.ProcessUVs();
        }

        private void ProcessVertices(RoomType roomType)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            foreach (string code in roomType.coordinatesBase64s)
            {
                var byteArr = System.Convert.FromBase64String(code);
                var floatArray = new float[byteArr.Length / 4];
                for (var i = 0; i < floatArray.Length; i++)
                {
                    floatArray[i] = BitConverter.ToSingle(byteArr, i * 4);
                }
                
                for (var i = 0; i < floatArray.Length; i = i + 3)
                {
                    var vertice = new Vector3(floatArray[i], floatArray[i + 2], floatArray[i + 1]);
                    vertices.Add(vertice);
                    testVertices.Add(vertice);
                }

                for (int start = 0; start < vertices.Count; start += 3)
                {
                    triangles.Add(start + 0);
                    triangles.Add(start + 1);
                    triangles.Add(start + 2);
                }
            }
            
            var newMesh = new Mesh();
            newMesh.vertices = vertices.ToArray();
            newMesh.triangles = triangles.ToArray();
            newMesh.RecalculateNormals();
            MeshGenerator.AddMesh(newMesh);
        }

        private void OnDrawGizmos()
        {
            if (testVertices == null)
                return;

            for (var i = 0; i < testVertices.Count(); i++)
            {
                Gizmos.DrawSphere(testVertices[i], 1.0f);
            }
        }
    }
}