﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Is responsible for a single step of an animation
/// (Used to be WaypointScript)
/// </summary>
/// <remarks>
/// It can include a movement and other functionality, but maximum one of each
/// </remarks>
public class TalkWalkWaitWaypointScript : WaypointScript
{
    public Dialog dialog;

    // This speed multiplier will be multiplied by the speed of the subject and the multiplier of the waypointcollection
    public float SpeedMultiplier = 1;

    // If true, this waypoint won't include movement
    // Can be used to create conversations
    public bool noMoving = true;

    public float WaitSeconds;

    /// <summary>
    /// Is responsible for animating this waypoint
    /// </summary>
    /// <param name="subject">The subject this waypoints animates</param>
    /// <param name="speed">The combined speed of the subject with the speed multiplier of the waypoint collection</param>
    /// <returns></returns>
    protected override IEnumerator AnimateInstance(AnimatableEntity subject, float speed)
    {
        if (!noMoving) 
            yield return subject.MoveTo(transform.position, speed * SpeedMultiplier);

        yield return new WaitForSeconds(WaitSeconds);

        if (dialog.sentences.Count > 0)
        {
            DialogManager dialogManager = DialogManager.Instance;
            yield return dialogManager.StartDialog(dialog);
        }

        IsDone = true;
    }

    protected override void DrawGizmosInstance(Transform prevWaypoint)
    {

        if (noMoving)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(prevWaypoint.position, transform.position);
        }
        else
        {
            DrawWalkGizmos();
        }
    }


    /// <summary>
    /// This will make sure that there will only be an arrow drawn from where the character was before
    /// This is not nice code, but is necessary to work around legacy
    /// </summary>
    private void DrawWalkGizmos()
    {
        WaypointCollectionScript wpcs = GetComponentInParent<WaypointCollectionScript>();
        Transform prevWaypoint = (wpcs.Subject && !wpcs.FromTrigger) ? wpcs.Subject.transform : wpcs.transform.parent;

        foreach (TalkWalkWaitWaypointScript talkWalkWaitWaypointScript in wpcs.GetComponentsInChildren<TalkWalkWaitWaypointScript>())
        {
            if (talkWalkWaitWaypointScript.gameObject.Equals(gameObject)) break;
            if (!talkWalkWaitWaypointScript.noMoving) prevWaypoint = talkWalkWaitWaypointScript.transform;
        }

        GizmosArrow.Draw(prevWaypoint.position, transform.position, Color.blue);
    }
}

