(ns tushi.board
  (:use tushi.interop)
  (:import [UnityEngine Application]))

; TODO: Profiling and logging with timbre
; TODO: Debug logging which is disabled in release

(def save-file (str Application/persistentDataPath "/save"))

(defn save!
  [this]
  ; TODO: Only save colors
  (spit save-file (state this)))

(defn load!
  [this]
  ; TODO: Verify file exists
  (state! this (read-string (slurp save-file))))

(defn start
  [this]
  (let [board (object-named "board")
        children (get-components-in-children board Board.Tile)]
    (swap-state! this #(assoc % :children children))))
