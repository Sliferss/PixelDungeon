using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float MoveSpeed = 5f;

    public void FollowPath(List<GridPosition> path)
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(path));
    }

    private IEnumerator MoveRoutine(List<GridPosition> path)
    {
        foreach (var pos in path)
        {
            Vector3 target = GridManager.Instance.GridToWorld(pos);

            while ((transform.position - target).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    MoveSpeed * Time.deltaTime
                );
                yield return null;
            }
        }
    }
}