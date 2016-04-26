(ns tushi.tile
  (:require [tushi.debug :as debug])
  (:use tushi.interop)
  (:import [UnityEngine Debug]))

; TODO: line ns?
(defn complete-line?
  [line]
  true)

(defn clear!
  [element]
  (-> element
      (get-component UnityEngine.UI.Image)
      .color
      (set! UnityEngine.Color/red)))

(defn on-click!
  [this data]
  (debug/log "clicked: " data)
  (let [s (state this)
        complete (filter complete-line? (:lines s))]
    (doseq [line complete
            element line]
      (clear! (:element element)))))
