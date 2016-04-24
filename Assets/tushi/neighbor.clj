(ns tushi.neighbor
  (:use tushi.interop)
  (:import [UnityEngine Application Debug]))

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
