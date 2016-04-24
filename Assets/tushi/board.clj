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
                         (into []
                               (keep-indexed
                                 (fn [index elem]
                                   {:element elem
                                    :position {:x index
                                               :y y}})
                                 (take row-width remaining-children))))]
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

(defn at-position
  [rows x y]
  (nth (nth rows y) x))

(defn bottom-half?
  [face-length y]
  (>= y face-length))

(defn row-width
  [face-length y]
  (if (bottom-half? face-length y)
    (+ face-length (- (- (* 2 face-length) 2) y)) ; Mirrored
    (+ face-length y)))

(defn top-left
  [rows face-length x y]
  (if (bottom-half? face-length y)
    (at-position rows x (- y 1))
    (when (and (> y 0) (> x 0)) ; Inside top left edge
      (at-position rows (- x 1) (- y 1)))))

(defn top-right
  [rows face-length x y]
  (if (bottom-half? face-length y)
    (at-position rows (+ 1 x) (- y 1))
    (when (and (> y 0)
               (< x (- (row-width face-length y) 1))) ; Inside top right edge
      (at-position rows x (- y 1)))))

(defn right
  [rows face-length x y]
  (if (< x (- (row-width face-length y) 1))
    (at-position rows (+ 1 x) y)))

(defn bind-neighbor
  [rows face-length elem]
  (let [pos (:position elem)
        tl (top-left rows face-length (:x pos) (:y pos))
        tr (top-right rows face-length (:x pos) (:y pos))
        r (right rows face-length (:x pos) (:y pos))]
    (assoc elem
           :top-left tl
           :top-right tr
           :right r)))

(defn bind-neighbors
  [rows face-length]
  (into []
        (map #(into []
                    (map (partial bind-neighbor rows face-length) %))
             rows)))

(defn start-hook
  [this]
  (let [board (object-named "board")
        children (get-components-in-children board Board.Tile)]
    (Debug/Log (count children))
    (swap-state! this #(assoc % :children children))

    (let [rows (hex-rows children face-length)]
      )))
