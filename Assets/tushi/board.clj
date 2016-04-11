(ns tushi.board
  (:use arcadia.core
        tushi.interop)
  (:import [UnityEngine Debug]))

(defn start-hook
  [this]
  (let [board (object-named "board")
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
