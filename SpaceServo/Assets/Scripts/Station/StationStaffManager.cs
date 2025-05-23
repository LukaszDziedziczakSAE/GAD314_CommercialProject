using System;
using System.Collections.Generic;
using UnityEngine;

public class StationStaffManager : MonoBehaviour
{
    [SerializeField] private StaffMember staffMemberPrefab;
    [field: SerializeField] public List<StaffMember> Members { get; private set; } = new List<StaffMember>();

    public event Action<PlaceableObject, StaffMember> OnNewHire;

    public StaffMember SpawnNew(PlaceableObject placeable,Transform spawnTransform)
    {
        Vector3 spawnLocation = new Vector3(0, 500, 0);
        //StaffMember newStaffMember = Instantiate(staffMemberPrefab, spawnLocation, Quaternion.identity);

        StaffMember newStaffMember = Instantiate(staffMemberPrefab, spawnTransform);
        Members.Add(newStaffMember);
        newStaffMember.SetName("StaffMember_" + Members.Count.ToString("D3"));
        OnNewHire?.Invoke(placeable, newStaffMember);
        return newStaffMember;
    }

    public void DespawnMember(StaffMember staffMember)
    {
        Members.Remove(staffMember);
    }
}
