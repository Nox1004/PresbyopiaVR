using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrebyopiaVR
{
    [RequireComponent(typeof(ViveController))]
    public class LaserPointer : MonoBehaviour
    {
        [SerializeField, Header("레이저 프리팹")]
        private GameObject _LaserPrefab;

        private GameObject _laser;
        private Transform _laserTransform;
        private Vector3 _hitPoint;

        [SerializeField, Header("레이 접촉 컬러")]
        private Color _contactColor;
        private Color _originColor;

        private ViveController _viveController;

        private void Awake()
        {
            _viveController = GetComponent<ViveController>();
        }

        private void Start()
        {
            _laser = Instantiate(_LaserPrefab);
            _laserTransform = _laser.transform;
        }

        private void ShowLaser(ref RaycastHit hit)
        {
            _laserTransform.position = Vector3.Lerp(_viveController.trackedObj.transform.position,
                                                    _hitPoint,
                                                    0.5f);
            _laserTransform.LookAt(_hitPoint);
            _laserTransform.localScale = new Vector3(_laserTransform.localScale.x,
                                                     _laserTransform.localScale.y,
                                                     hit.distance);
        }

        /// <summary>
        /// 레이케스트에 접촉한 컬라이더 검사하기
        /// </summary>
        private void ExamineCollider(ref RaycastHit hit)
        {
            var contactCol = _viveController.contactCollider;

            if (hit.transform != null)
            {
                if (hit.collider.material != null)
                {
                    contactCol = hit.collider;

                    var mat = hit.collider.GetComponent<Renderer>().material;

                    if (mat.color != _contactColor)
                        _originColor = mat.color;

                    mat.color = _contactColor;
                }
            }
            else 
            {
                if (contactCol != null)
                {
                    contactCol.GetComponent<Renderer>().material.color = _originColor;
                    contactCol = null;
                }
            }
        }

        private void Update()
        {
            RaycastHit hit;

            if (_viveController.objectinHand == null)
            {
                if (Physics.Raycast(_viveController.trackedObj.transform.position,
                                    transform.forward,
                                    out hit,
                                    10000,
                                    GameManager.Instance.layerMask))
                {
                    _hitPoint = hit.point;

                    ShowLaser(ref hit);

                    ExamineCollider(ref hit);
                }
            }
            else
            {
                if (Physics.Raycast(_viveController.trackedObj.transform.position,
                                    transform.forward,
                                    out hit,
                                    10000))
                {
                    _hitPoint = hit.point;

                    ShowLaser(ref hit);
                }
            }
        }
    }
}