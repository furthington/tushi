(ns tushi.neighbor
  (:require [arcadia.core :as arcadia]
            [tushi.debug :as debug])
  (:use tushi.interop)
  (:import [UnityEngine]))

(def face-length 5)

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

(defn top-half?
  [face-length y]
  (< y (- face-length 1)))

(defn row-width
  [face-length y]
  (if (bottom-half? face-length y)
    (+ face-length (- (- (* 2 face-length) 2) y)) ; Mirrored
    (+ face-length y)))

(defn top-left
  [rows face-length x y]
  (if (bottom-half? face-length y)
    (at-position rows x (- y 1))
    (when (and (> y 0) (> x 0))
      (at-position rows (- x 1) (- y 1)))))

(defn top-right
  [rows face-length x y]
  (if (bottom-half? face-length y)
    (at-position rows (+ 1 x) (- y 1))
    (when (and (> y 0)
               (< x (- (row-width face-length y) 1)))
      (at-position rows x (- y 1)))))

(defn right
  [rows face-length x y]
  (if (< x (- (row-width face-length y) 1))
    (at-position rows (+ 1 x) y)))

(defn left
  [rows face-length x y]
  (if (> x 0)
    (at-position rows (- x 1) y)))

(defn bottom-right
  [rows face-length x y]
  (if (top-half? face-length y)
    (at-position rows (+ 1 x) (+ 1 y))
    (when (and (< y (- (* 2 face-length) 2))
               (< x (- (row-width face-length y) 1)))
      (at-position rows x (+ 1 y)))))

(defn bottom-left
  [rows face-length x y]
  (if (top-half? face-length y)
    (at-position rows x (+ 1 y))
    (when (and (< y (- (* 2 face-length) 2))
               (> x 0))
      (at-position rows (- x 1) (+ 1 y)))))

(defn meet
  [rows face-length elem]
  (let [pos (:position elem)
        x (:x pos)
        y (:y pos)
        l (left rows face-length x y)
        tl (top-left rows face-length x y)
        tr (top-right rows face-length x y)
        r (right rows face-length x y)
        br (bottom-right rows face-length x y)
        bl (bottom-left rows face-length x y)]
    (assoc elem
           :left l
           :top-left tl
           :top-right tr
           :right r
           :bottom-right br
           :bottom-left bl)))

(defn introduce
  [rows face-length]
  (into []
        (map #(into []
                    (map (partial meet rows face-length) %))
             rows)))

(defn follow
  [rows element dir]
  (loop [acc []
         el element]
    (if-let [x (dir el)]
      (recur (conj acc x) (at-position rows
                                       (:x (:position x))
                                       (:y (:position x))))
      acc)))

(defn build-line
  [rows element start-dir end-dir]
  (concat (follow rows element start-dir)
          [element]
          (follow rows element end-dir)))

(defn build-lines
  [rows]
  (map (fn [row]
         (map #(assoc %
                      :lines (map (partial build-line rows %)
                                  [:top-left :bottom-left :left]
                                  [:bottom-right :top-right :right]))
              row))
       rows))

(defn apply-to-editor!
  []
  ;(assert (arcadia/editor?) "must be in editor")
  (let [board (object-named "board")
        children (get-components-in-children board Board.Tile)
        rows (hex-rows children face-length)
        neighbored (introduce rows face-length)
        lined (build-lines neighbored)]
    (doseq [row lined
            item row]
      (let [tile (-> (:element item)
                     (get-component Board.Tile))
            elem #(:element (% item))]

        (set! (.top_right tile) (elem :top-right))
        (set! (.top_left tile) (elem :top-left))
        (set! (.left tile) (elem :left))
        (set! (.right tile) (elem :right))
        (set! (.bottom_right tile) (elem :bottom-right))
        (set! (.bottom_left tile) (elem :bottom-left))
        (.Clear (.flat_lines tile))
        (doseq [line (:lines item)]
          (doseq [el line]
            (.Add (.flat_lines tile) (:element el)))
          (.Add (.flat_lines tile) nil))
        (println "lines: " (.Count (.flat_lines tile)))
        ))))
