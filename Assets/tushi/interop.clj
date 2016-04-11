(ns tushi.interop
  (:use arcadia.core)
  (:import [UnityEngine Debug]))

(defn get-components
  ([^UnityEngine.GameObject go]
   (.GetComponents go UnityEngine.Component))

  ([^UnityEngine.GameObject go ^System.Type t]
   (.GetComponents go t)))

(defn get-components-in-children
  ([^UnityEngine.GameObject go]
   (.GetComponentsInChildren go UnityEngine.Component))

  ([^UnityEngine.GameObject go ^System.Type t]
   (.GetComponentsInChildren go t)))
