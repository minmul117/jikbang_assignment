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
        Root Response;

        public MeshGenerator MeshGenerator;
        
        List<Vector3> testVertices;

        void Start()
        {
            testVertices = new List<Vector3>();
            SendRequest();
        }

        void Update()
        {

        }

        public void SendRequest()
        {
            // NOTE(minmul117): API 리퀘스트를 던지고 응답을 받는 부분.
            // 원래대로라면 응답 던지고 / 받는 부분 둘 다 이 클래스에서 분리되어 있어야 하나...
            // 실제로 응답을 가져오지는 않으니 지금은 패스한다.
            var resourcePath = "Samples/json/dong";
            var loadedAsset = Resources.Load<TextAsset>(resourcePath);

            var Parser = new ResponseParser.Parser();
            Response = Parser.ParseResponse(loadedAsset.text);
            
            if (Response.Success && Response.Code == 200)
            {
                DrawVertices(Response);
            }
        }

        private void DrawVertices(Root Response)
        {
            foreach (DongData dongData in Response.Data)
            {
                var dongName = dongData.Meta.Dong;
                // 한 동의 verticese는 동 데이터 안에 있다.
                foreach (RoomType roomType in dongData.RoomTypes)
                {
                    ProcessVertices(roomType);
                }
            }
        }

        private void ProcessVertices(RoomType roomType)
        {
            foreach (string code in roomType.CoordinatesBase64s)
            {
                List<Vector3> vertices = new List<Vector3>();
                List<int> triangles = new List<int>();

                Byte[] byteArr = System.Convert.FromBase64String(code);
                float[] floatArray = new float[byteArr.Length / 4];
                for (int i = 0; i < floatArray.Length; i++)
                {
                    floatArray[i] = BitConverter.ToSingle(byteArr, i * 4);
                }
                
                for (int i = 0; i < floatArray.Length; i = i + 3)
                {
                    var vertice = new Vector3(floatArray[i], floatArray[i + 2], floatArray[i + 1]);
                    vertices.Add(vertice);
                    if (vertices.Count >= 4) 
                    {
                        // Completed a new quad, create 2 triangles
                        // Unity uses a 'Clockwise Winding Order' for determining front-facing polygons.
                        int start = vertices.Count - 4;
                        triangles.Add(start + 0);
                        triangles.Add(start + 1);
                        triangles.Add(start + 2);
                        triangles.Add(start + 1);
                        triangles.Add(start + 3);
                        triangles.Add(start + 2);
                    }
                    testVertices.Add(vertice);
                }
                // 두 개가 있는데... 각자 다른 메쉬겠지?
                // 벡터 리스트 하나당 메시 하나씩 할당해야 한다.
                Mesh newMesh = new Mesh();
                newMesh.vertices = vertices.ToArray();
                newMesh.triangles = triangles.ToArray();
                MeshGenerator.AddMesh(newMesh);
            }
        }

        private void OnDrawGizmos()
        {
            if (testVertices == null)
                return;

            for (int i = 0; i < testVertices.Count(); i++)
            {
                Gizmos.DrawSphere(testVertices[i], 1.0f);
            }
        }
    }
}