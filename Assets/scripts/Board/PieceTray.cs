﻿using UnityEngine;
        if(Random.Range(0.0f, 1.0f) <= 0.2f)
        { Instantiate(PrefabsLowProb[Random.Range(0, PrefabsLowProb.Count)]).transform.SetParent(transform, false); }
        else
        { Instantiate(PrefabsHighProb[Random.Range(0, PrefabsHighProb.Count)]).transform.SetParent(transform, false); }
      }