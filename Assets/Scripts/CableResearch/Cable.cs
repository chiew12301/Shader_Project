using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace WIRECABLE
{
    public class Cable : MonoBehaviour
    {

        [Header("Look")]
        [SerializeField, Min(1)] private int m_numberOfPoints = 3;
        [SerializeField, Min(0.01f)] private float m_space = 0.3f;
        [SerializeField, Min(0.1f)] private float m_size = 0.3f;

        [Header("Behaviour")]
        [SerializeField, Min(1.0f)] private float m_springForce = 200;
        [SerializeField, Min(1.0f)] private float m_braketLengthMultiplier = 2.0f;
        [SerializeField, Min(0.1f)] private float m_minBrakeTime = 1.0f;
        private float m_brakeLength = 1.0f;
        private float m_timeToBrake = 1.0f;

        [Header("References")]
        [SerializeField, Required] private GameObject m_startPoint;
        [SerializeField, Required] private GameObject m_endPoint;
        [SerializeField, Required] private GameObject m_connector;
        [SerializeField, Required] private GameObject m_point;

        private List<Transform> m_pointList = new List<Transform>();
        private List<Transform> m_connectorList = new List<Transform>();
        private const string m_cloneText = "Part";
        private Connector m_startConnector = null;
        private Connector m_endConnector = null;

        public Connector StartConnector => this.m_startConnector;
        public Connector EndConnector => this.m_endConnector;
        public IReadOnlyList<Transform> Points => this.m_pointList;
        
        //==================================================

        private void Start()
        {
            this.m_startConnector = this.m_startPoint.GetComponent<Connector>();
            this.m_endConnector = this.m_endPoint.GetComponent<Connector>();

            this.m_brakeLength = this.m_space * this.m_numberOfPoints * this.m_braketLengthMultiplier + 2f;

            this.m_pointList = new List<Transform>();
            this.m_connectorList = new List<Transform>();

            this.m_pointList.Add(this.m_startPoint.transform);
            this.m_pointList.Add(this.m_point.transform);

            this.m_connectorList.Add(this.m_connector.transform);

            for (int i = 1; i < this.m_numberOfPoints; i++)
            {
                Transform conn = GetConnector(i);
                if (conn == null)
                {
                    Debug.LogWarning("Can't found connector number " + i);
                }
                else
                {
                    this.m_connectorList.Add(conn);
                }

                Transform point = GetPoint(i);
                if (conn == null)
                {
                    Debug.LogWarning("Can't found point number " + i);
                }
                else
                {
                    this.m_pointList.Add(point);
                }
            }

            Transform endConn = GetConnector(this.m_numberOfPoints);
            if (endConn == null)
            {
                Debug.LogWarning("Can't found connector number " + this.m_numberOfPoints);
            }
            else
            {
                this.m_connectorList.Add(endConn);
            }

            this.m_pointList.Add(this.m_endPoint.transform);
        }

        private void Update()
        {
            float cableLength = 0f;
            bool isConnected = this.m_startConnector.IsConnected || this.m_endConnector.IsConnected;

            int numOfParts = this.m_connectorList.Count;
            Transform lastPoint = this.m_pointList[0];
            for (int i = 0; i < numOfParts; i++)
            {
                Transform nextPoint = this.m_pointList[i + 1];
                Transform connector = this.m_connectorList[i].transform;
                connector.position = this.CountConPos(lastPoint.position, nextPoint.position);
                if (lastPoint.position == nextPoint.position || nextPoint.position == connector.position)
                {
                    connector.localScale = Vector3.zero;
                }
                else
                {
                    connector.rotation = Quaternion.LookRotation(nextPoint.position - connector.position);
                    connector.localScale = this.CountSizeOfCon(lastPoint.position, nextPoint.position);
                }

                if (isConnected)
                {
                    cableLength += (lastPoint.position - nextPoint.position).magnitude;
                }

                lastPoint = nextPoint;
            }

            if (isConnected)
            {
                if (cableLength > this.m_brakeLength)
                {
                    this.m_timeToBrake -= Time.deltaTime;
                    if (this.m_timeToBrake < 0f)
                    {
                        this.m_startConnector.Disconnect();
                        this.m_endConnector.Disconnect();
                        this.m_timeToBrake = this.m_minBrakeTime;
                    }
                }
                else
                {
                    this.m_timeToBrake = this.m_minBrakeTime;
                }
            }
        }

        //==================================================

        private string ConnectorName(int index) => $"{m_cloneText}_{index}_Conn";
        private string PointName(int index) => $"{m_cloneText}_{index}_Point";

        private Vector3 CountNewPointPos(Vector3 lastPos) => lastPos + this.transform.forward * this.m_space;
        private Vector3 CountConPos(Vector3 start, Vector3 end) => (start + end) / 2f;
        private Vector3 CountSizeOfCon(Vector3 start, Vector3 end) => new Vector3(this.m_size, this.m_size, (start - end).magnitude / 2f);
        private Quaternion CountRoationOfCon(Vector3 start, Vector3 end) => Quaternion.LookRotation(end - start, Vector3.right);
        private Transform GetConnector(int index) => index > 0 ? transform.Find(ConnectorName(index)) : this.m_connector.transform;
        private Transform GetPoint(int index) => index > 0 ? transform.Find(PointName(index)) : this.m_point.transform;


        [Button("Reset Points")]
        private void UpdatePoints()
        {
            if(!this.m_startPoint || !this.m_endPoint || !this.m_point || !this.m_connector)
            {
                Debug.LogWarning("References is missing! Please check inspector!");
                return;
            }

            int length = this.transform.childCount;
            for(int i = 0; i < length; i++)
            {
                if(this.transform.GetChild(i).name.StartsWith(m_cloneText))
                {
                    DestroyImmediate(this.transform.GetChild(i).gameObject);
                    length--;
                    i--;
                }
            }
            
            Vector3 lastPosition = this.m_startPoint.transform.position;
            Rigidbody lastRb = this.m_startPoint.GetComponent<Rigidbody>();

            if(lastRb == null)
            {
                Debug.LogWarning("Not found Rigidbody on your start point! Please check!");
                return;
            }

            for(int i = 0; i < this.m_numberOfPoints; i++)
            {
                GameObject cConnector = i == 0 ? this.m_connector : this.CreateNewCon(i);
                GameObject cPoint = i == 0 ? this.m_point :this.CreateNewPoint(i);

                Vector3 newPosition = this.CountNewPointPos(lastPosition);
                cPoint.transform.position = newPosition;
                cPoint.transform.localScale = Vector3.one * this.m_size;
                cPoint.transform.rotation = transform.rotation;

                this.SetSpirng(cPoint.GetComponent<SpringJoint>(), lastRb);

                lastRb = cPoint.GetComponent<Rigidbody>();

                cConnector.transform.position = this.CountConPos(lastPosition, newPosition);
                cConnector.transform.localScale = this.CountSizeOfCon(lastPosition, newPosition);
                cConnector.transform.rotation = this.CountRoationOfCon(lastPosition, newPosition);
                lastPosition = newPosition;
            }

            Vector3 endPosition = this.CountNewPointPos(lastPosition);
            this.m_endPoint.transform.position = endPosition;
            this.SetSpirng(lastRb.gameObject.AddComponent<SpringJoint>(), this.m_endPoint.GetComponent<Rigidbody>());

            GameObject endConnector = CreateNewCon(this.m_numberOfPoints);
            endConnector.transform.position = this.CountConPos(lastPosition, endPosition);
            endConnector.transform.rotation = this.CountRoationOfCon(lastPosition, endPosition);
        }

        [Button("Add point")]
        private void AddPoint()
        {
            Transform lastprevPoint = GetPoint(this.m_numberOfPoints - 1);
            if (lastprevPoint == null)
            {
                Debug.LogWarning("Can't found point number " + (this.m_numberOfPoints - 1));
                return;
            }

            Rigidbody endRB = this.m_endConnector.GetComponent<Rigidbody>();
            foreach (var spring in lastprevPoint.GetComponents<SpringJoint>())
            {
                if (spring.connectedBody == endRB)
                {
                    DestroyImmediate(spring);                    
                }
            }

            GameObject cPoint = CreateNewPoint(this.m_numberOfPoints);
            GameObject cConnector = CreateNewCon(this.m_numberOfPoints + 1);

            cPoint.transform.position = this.m_endPoint.transform.position;
            cPoint.transform.rotation = this.m_endPoint.transform.rotation;
            cPoint.transform.localScale = Vector3.one * this.m_size;

            SetSpirng(cPoint.GetComponent<SpringJoint>(), lastprevPoint.GetComponent<Rigidbody>());
            SetSpirng(cPoint.AddComponent<SpringJoint>(), endRB);

            // fix end
            this.m_endPoint.transform.position += this.m_endPoint.transform.forward * this.m_space;

            cConnector.transform.position = CountConPos(cPoint.transform.position, this.m_endPoint.transform.position);
            cConnector.transform.localScale = CountSizeOfCon(cPoint.transform.position, this.m_endPoint.transform.position);
            cConnector.transform.rotation = CountRoationOfCon(cPoint.transform.position, this.m_endPoint.transform.position);

            this.m_numberOfPoints++;
        }

        [Button("Remove point")]
        private void RemovePoint()
        {
            if (this.m_numberOfPoints < 2)
            {
                Debug.LogWarning("Cable can't be shorter then 1");
                return;
            }

            Transform lastprevPoint = GetPoint(this.m_numberOfPoints - 1);
            if (lastprevPoint == null)
            {
                Debug.LogWarning("Can't found point number " + (this.m_numberOfPoints - 1));
                return;
            }

            Transform lastprevCon = GetConnector(this.m_numberOfPoints);
            if (lastprevCon == null)
            {
                Debug.LogWarning("Can't found connector number " + (this.m_numberOfPoints));
                return;
            }

            Transform lastlastprevPoint = GetPoint(this.m_numberOfPoints - 2);
            if (lastlastprevPoint == null)
            {
                Debug.LogWarning("Can't found point number " + (this.m_numberOfPoints - 2));
                return;
            }


            Rigidbody endRB = this.m_endPoint.GetComponent<Rigidbody>();
            SetSpirng(lastlastprevPoint.gameObject.AddComponent<SpringJoint>(), endRB);

            this.m_endPoint.transform.position = lastprevPoint.position;
            this.m_endPoint.transform.rotation = lastprevPoint.rotation;

            DestroyImmediate(lastprevPoint.gameObject);
            DestroyImmediate(lastprevCon.gameObject);

            this.m_numberOfPoints--;
        }

        public void SetSpirng(SpringJoint spring, Rigidbody connectedBody)
        {
            spring.connectedBody = connectedBody;
            spring.spring = this.m_springForce;
            spring.damper = 0.2f;
            spring.autoConfigureConnectedAnchor = false;
            spring.anchor = Vector3.zero;
            spring.connectedAnchor = Vector3.zero;
            spring.minDistance = this.m_space;
            spring.maxDistance = this.m_space;
        }

        private GameObject CreateNewPoint(int index)
        {
            GameObject temp = Instantiate(this.m_point);
            temp.name = PointName(index);
            temp.transform.parent = transform;
            return temp;
        }
        private GameObject CreateNewCon(int index)
        {
            GameObject temp = Instantiate(this.m_connector);
            temp.name = ConnectorName(index);
            temp.transform.parent = transform;
            return temp;
        }

        //==================================================
    }
}