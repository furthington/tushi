(ns tushi.tile
  (:require [tushi.debug :as debug])
  (:use tushi.interop)
  (:import [UnityEngine Debug]))

(defn on-click
  [this data]
  (debug/log "clicked: " data))
