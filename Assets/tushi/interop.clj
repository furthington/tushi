(ns tushi.interop
  (:use arcadia.core)
  (:import [UnityEngine Debug]))

(defn get-components
  ([go]
   (.GetComponents go UnityEngine.Component))
  ([go t]
   (.GetComponents go t)))

(defn get-components-in-children
  ([go]
   (.GetComponentsInChildren go UnityEngine.Component))
  ([go t]
   (.GetComponentsInChildren go t)))

(defn get-children
  [go]
  (let [transform (get-component go UnityEngine.Transform)]
    ; Transform acts as a collection of its children, so we
    ; can easily pull them out with identity
    (map identity transform)))
