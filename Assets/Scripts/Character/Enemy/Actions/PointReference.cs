using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public enum PointReferenceTypes
{
    Breath,
    Projectile
}
public class PointReference : MonoBehaviour
{
    public PointReferenceTypes PointType;
}
