(ns tushi.board
  (:use arcadia.core
        tushi.interop)
  (:import [UnityEngine Debug]))

(def long-width 10)
(def short-width 9)
(def height 10)

; TODO: Load/save state
; TODO: Find neighbors

(defn start-hook
  [this]
  (let [board (object-named "board")
        all-children (get-components-in-children board Board.Tile)]
    (Debug/Log (count all-children))))
