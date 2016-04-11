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
        ; TODO: Add a component to each hex and just use this fn?
        groups (get-components-in-children
                 board
                 UnityEngine.UI.HorizontalLayoutGroup)
        all-children (loop [remaining groups
                            all '()]
                       (if (empty? remaining)
                         all
                         (recur (rest remaining)
                                (concat all (get-children
                                              (first remaining))))))]
    (Debug/Log (count all-children))))
