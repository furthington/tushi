(ns tushi.board
  (:use arcadia.core
        tushi.interop)
  (:import [UnityEngine Application Debug]
           ArcadiaState))

(def face-length 5)
(def save-file (str Application/persistentDataPath "/save"))

(defn save!
  [this]
  ; TODO: Only save colors
  (spit save-file (state this)))

(defn load!
  [this]
  ; TODO: Verify file exists
  (state! this (read-string (slurp save-file))))

; Find neighbors
;   given face length
;     find rows
;   with each tile and its index
;     calculate its row
;     use row info to find its neighbors

(defn hex-rows
  [children face-length]
  (loop [rows []
         remaining-children children
         row-width face-length
         top-half? true]
    (let [half-way? (= row-width (- (* 2 face-length) 1))
          y (count rows)
          new-rows (conj rows
                         (keep-indexed
                           (fn [index elem]
                             {:element elem
                              :position [index y]})
                           (take row-width remaining-children)))]
      (if (and (not top-half?) (= row-width face-length))
        new-rows
        (recur new-rows
               (drop row-width remaining-children)
               (if (and top-half? (not half-way?))
                 (+ 1 row-width)
                 (- row-width 1))
               (if half-way?
                 (not top-half?)
                 top-half?))))))

(defn start-hook
  [this]
  (let [board (object-named "board")
        children (get-components-in-children board Board.Tile)]
    (Debug/Log (count children))
    (swap-state! this #(assoc % :children children))

    (let [rows (hex-rows children face-length)]
      )))
