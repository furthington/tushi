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
    ; can easily pull them out
    (seq transform)))

(defn ensure-component
  [go c]
  (or (.GetComponent go c)
      (.AddComponent go c)))

(defn state!
  [go s]
  (let [c (ensure-component go ArcadiaState)]
    (set! (.state c) s)))

(defn state
  [go]
  (let [c (ensure-component go ArcadiaState)]
    (.state c)))

(defn swap-state!
  [go f & args]
  (state! go (apply f (state go) args)))
