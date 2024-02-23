// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace WIRECABLE
// {
//     [RequireComponent(typeof(Rigidbody))]
//     public class Connector : MonoBehaviour
//     {
//         public enum ConnectorType { Male, Female }
//         public enum CableColor { White, Red, Green, Yellow, Blue, Cyan, Magenta }

//         [field: Header("Settings")]

//         [field: SerializeField] public ConnectorType ConnectionType { get; private set; } = ConnectorType.Male;
//         [field: SerializeField, OnValueChanged(nameof(UpdateConnectorColor))] public CableColor ConnectionColor { get; private set; } = CableColor.White;

//         [SerializeField] private bool m_makeConnectionKinematic = false;
//         private bool m_wasConnectionKinematic;

//         [SerializeField] private bool m_hideInteractableWhenIsConnected = false;
//         [SerializeField] private bool m_allowConnectDifrentCollor = false;

//         [field: SerializeField] public Connector ConnectedTo { get; private set; }


//         [Header("Object to set")]
//         [SerializeField, Required] private Transform m_connectionPoint;
//         [SerializeField] private MeshRenderer m_collorRenderer;
//         [SerializeField] private ParticleSystem m_sparksParticle;


//         private FixedJoint m_fixedJoint;
//         public Rigidbody Rigidbody { get; private set; }

//         public Vector3 ConnectionPosition => m_connectionPoint ? m_connectionPoint.position : transform.position;
//         public Quaternion ConnectionRotation => m_connectionPoint ? m_connectionPoint.rotation : transform.rotation;
//         public Quaternion RotationOffset => m_connectionPoint ? m_connectionPoint.localRotation : Quaternion.Euler(Vector3.zero);
//         public Vector3 ConnectedOutOffset => m_connectionPoint ? m_connectionPoint.right : transform.right;

//         public bool IsConnected => ConnectedTo != null;
//         public bool IsConnectedRight => IsConnected && ConnectionColor == ConnectedTo.ConnectionColor;

//         private IEnumerator m_incorrectSparksC;

//         private void Awake()
//         {
//             this.Rigidbody = this.gameObject.GetComponent<Rigidbody>();
//         }

//         private void Start()
//         {
//             this.UpdateConnectorColor();

//             if (this.ConnectedTo != null)
//             {
//                 Connector t = this.ConnectedTo;
//                 this.ConnectedTo = null;
//                 this.Connect(t);
//             }
//         }

//         private void OnDisable() => this.Disconnect();

//         public void SetAsConnectedTo(Connector secondConnector)
//         {
//             this.ConnectedTo = secondConnector;
//             this.m_wasConnectionKinematic = secondConnector.Rigidbody.isKinematic;
//             UpdateInteractableWhenIsConnected();
//         }
//         public void Connect(Connector secondConnector)
//         {
//             if (secondConnector == null)
//             {
//                 Debug.LogWarning("Attempt to connect null");
//                 return;
//             }

//             if (this.IsConnected)
//                 this.Disconnect(secondConnector);

//             secondConnector.transform.rotation = this.ConnectionRotation * secondConnector.RotationOffset;
//             secondConnector.transform.position = this.ConnectionPosition - (secondConnector.ConnectionPosition - secondConnector.transform.position);

//             this.m_fixedJoint = this.gameObject.AddComponent<FixedJoint>();
//             this.m_fixedJoint.connectedBody = secondConnector.Rigidbody;

//             secondConnector.SetAsConnectedTo(this);
//             this.m_wasConnectionKinematic = secondConnector.Rigidbody.isKinematic;
//             if (this.m_makeConnectionKinematic)
//                 secondConnector.Rigidbody.isKinematic = true;
//             this.ConnectedTo = secondConnector;

//             // sparks on inncretc connection
//             if (this.m_incorrectSparksC == null && this.m_sparksParticle && this.IsConnected && !this.IsConnectedRight)
//             {
//                 this.m_incorrectSparksC = IncorrectSparks();
//                 this.StartCoroutine(m_incorrectSparksC);
//             }

//             // disable outline on select
//             this.UpdateInteractableWhenIsConnected();
//         }
//         public void Disconnect(Connector onlyThis = null)
//         {
//             if (this.ConnectedTo == null || this.onlyThis != null && this.onlyThis != this.ConnectedTo)
//                 return;

//             this.Destroy(this.m_fixedJoint);

//             // important to dont make recusrion
//             Connector toDisconect = this.ConnectedTo;
//             this.ConnectedTo = null;
//             if (this.m_makeConnectionKinematic)
//                 this.toDisconect.Rigidbody.isKinematic = this.m_wasConnectionKinematic;
//             this.toDisconect.Disconnect(this);

//             // sparks on inncretc connection
//             if (this.m_sparksParticle)
//             {
//                 this.m_sparksParticle.Stop();
//                 this.m_sparksParticle.Clear();
//             }

//             // enable outline on select
//             this.UpdateInteractableWhenIsConnected();
//         }

//         private void UpdateInteractableWhenIsConnected()
//         {
//             if (this.m_hideInteractableWhenIsConnected)
//             {
//                 if (this.TryGetComponent(out Collider collider))
//                     collider.enabled = !this.IsConnected;
//             }
//         }

//         private IEnumerator IncorrectSparks()
//         {
//             while (this.m_incorrectSparksC != null && this.m_sparksParticle && this.IsConnected && !this.IsConnectedRight)
//             {
//                 this.m_sparksParticle.Play();

//                 yield return new WaitForSeconds(Random.Range(0.6f, 0.8f));
//             }
//             this.m_incorrectSparksC = null;
//         }

//         private void UpdateConnectorColor()
//         {
//             if (this.m_collorRenderer == null)
//                 return;

//             Color color = this.MaterialColor(this.ConnectionColor);
//             MaterialPropertyBlock probs = new();
//             this.m_collorRenderer.GetPropertyBlock(probs);
//             probs.SetColor("_Color", color);
//             this.m_collorRenderer.SetPropertyBlock(probs);
//         }

//         private Color MaterialColor(CableColor cableColor) => cableColor switch
//         {
//             CableColor.White => Color.white,
//             CableColor.Red => Color.red,
//             CableColor.Green => Color.green,
//             CableColor.Yellow => Color.yellow,
//             CableColor.Blue => Color.blue,
//             CableColor.Cyan => Color.cyan,
//             CableColor.Magenta => Color.magenta,
//             _ => Color.clear
//         };


//         public bool CanConnect(Connector secondConnector) =>
//             this != secondConnector
//             && !this.IsConnected && !secondConnector.IsConnected
//             && this.ConnectionType != secondConnector.ConnectionType
//             && (this.m_allowConnectDifrentCollor || secondConnector.m_allowConnectDifrentCollor || this.ConnectionColor == secondConnector.ConnectionColor);
//     }
// }