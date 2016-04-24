(ns tushi.editor
  (:use tushi.interop)
  (:require [arcadia.core :as arcadia]
            [tushi.neighbor :as neighbor])
  (:import [UnityEngine Application Debug]))

(defn apply-neighbors!
  []
  (assert (arcadia/editor?) "must be in editor")
  (let [board (object-named "board")
        children (get-components-in-children board Board.Tile)
        rows (neighbor/hex-rows children neighbor/face-length)
        neighbored (neighbor/introduce rows neighbor/face-length)]
    (doseq [row neighbored
            item row]
      (swap-state! (:element item)
                   #(assoc %
                           :left (:left item)
                           :top-left (:top-left item)
                           :top-right (:top-right item)
                           :right (:right item)
                           :bottom-right (:bottom-right item)
                           :bottom-left (:bottom-left item))))))
