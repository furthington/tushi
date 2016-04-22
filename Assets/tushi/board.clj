(ns tushi.board
  (:use arcadia.core
        tushi.interop)
  (:import [UnityEngine Application Debug]
           ArcadiaState))

(def long-width 10)
(def short-width 9)
(def height 10)
(def save-file (str Application/persistentDataPath "/save"))

; TODO: Find neighbors

(defn save!
  [this]
  ; TODO: Only save colors
  (spit save-file (state this)))

(defn load!
  [this]
  ; TODO: Verify file exists
  (state! this (read-string (slurp save-file))))

(defn start-hook
  [this]
  (let [board (object-named "board")
        children (get-components-in-children board Board.Tile)]
    (Debug/Log (count children))
    (swap-state! this #(assoc % :children children))))
